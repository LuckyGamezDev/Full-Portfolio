import datetime
import json
import os

import requests
from countryinfo import CountryInfo

from kivy.app import App

from kivy.factory import Factory

from kivy.uix.screenmanager import ScreenManager, Screen, RiseInTransition

from kivy.properties import NumericProperty

import BudgetTrackerWidgets

class Product():
    name: str
    cost: int
    description: str

    def __init__(self, productName, productCost, productDescription):
        self.name = productName
        self.cost = productCost
        self.description = productDescription

    def ToDictionary(self) -> dict:
        return {"name": self.name, "cost": self.cost, "description": self.description}

class ProductPurchaseHistory():
    purchasedProductsHistoryList: list[Product] = []

    def __init__(self, **kwargs):
        super().__init__(**kwargs)

        loadedProductsList = JsonHandler.LoadProductPurchaseHistoryDataListFromJson()
        if loadedProductsList is not None:
            self.purchasedProductsHistoryList = loadedProductsList
        else:
            print("No history yet available to load!")
    
    def AddProductPurchaseToHistory(self, product: Product):

        self.purchasedProductsHistoryList.append(product)

        self.SaveHistory()

    def ReloadHistory(self):
        loadedProductsList = JsonHandler.LoadProductPurchaseHistoryDataListFromJson()

        if loadedProductsList is not None:
            self.purchasedProductsHistoryList = loadedProductsList

    def SaveHistory(self):
        dataToSave = []
        for product in self.purchasedProductsHistoryList:
            dataToSave.append(product.ToDictionary())

        JsonHandler.SaveProductPurchaseHistoryDataToJson(dataToSave)

        print("Saving history...")

    def GetProductsList(self) -> list[Product]:
        return self.purchasedProductsHistoryList

class MainScreen(Screen):
    pass

class ProductPurchaseHistoryScreen(Screen):
    def on_enter(self):
        self.ids.history.UpdateFromApp(self)

class BudgetTrackerApp(App):
    initialBudgetAmount = None
    intialBudgetDuration = None

    budgetAmount = NumericProperty(0)

    userCurrency = None

    def build(self):
        screenManager = ScreenManager(transition=RiseInTransition())

        mainScreen = MainScreen(name="main")
        productHistoryScreen = ProductPurchaseHistoryScreen(name="product_history")

        screenManager.add_widget(mainScreen)
        screenManager.add_widget(productHistoryScreen)

        self.history = ProductPurchaseHistory()

        self.budgetEndDate, self.budgetAmount, self.initialBudgetAmount, self.initialBudgetDuration, self.hasBudgetBeenSet = JsonHandler.LoadBudgetFromJson()

        screenManager.current = "main"
        return screenManager

    def on_start(self):
        userIP, userCountryCode, result = JsonHandler.LoadUserData()

        if result is False:
            userIP = NetworkHandler.GetUserIP()
            userCountryCode = NetworkHandler.GetCountryCodeFromIP(userIP)

            JsonHandler.SaveUserData(userIP, userCountryCode)

            print(f"Found user country: {userCountryCode}")
        else:
            print("Found user data, loaded user IP and country!")

        # Look for the user's currency
        countryInfo = CountryInfo(userCountryCode)

        if countryInfo:
            print(f"User currency is: {countryInfo.currencies()}")

            self.userCurrency = countryInfo.currencies()[0] # Always assign the first currency of the list (even if there are more currencies than 1)

    def AddProduct(self, product: Product):
        # Subtract the cost of the product from the current budget
        self.budgetAmount -= int(product.cost)

        # Save the remaining budget here
        JsonHandler.SaveBudgetToJson(None, None, self.budgetEndDate, self.budgetAmount)
        print(f"Remaining budget: {self.budgetAmount}")

        self.history.AddProductPurchaseToHistory(product)

    def SetNewBudget(self, budgetAmount: int, monthsAmount: int):
        if monthsAmount > 0 and monthsAmount <= 12:
            budgetEndDateYear, budgetEndDateMonth = Factory.SetBudgetPopup.CalculateEndDate(monthsAmount)
                        
            self.budgetEndDate = datetime.datetime(budgetEndDateYear, budgetEndDateMonth, 1, 0, 0, 0)

            self.initialBudgetAmount = budgetAmount
            self.initialBudgetDuration = monthsAmount

        if budgetAmount > 0:
            self.budgetAmount = budgetAmount
            self.hasBudgetBeenSet = True

        JsonHandler.SaveBudgetToJson(budgetAmount, monthsAmount, self.budgetEndDate, self.budgetAmount)
        if JsonHandler.DeleteProductPurchaseHistoryData():
            self.history.ReloadHistory()
        else:
            print("Failed to delete history!")

    def CanAddProduct(self, productCost) -> bool:
        if productCost.isnumeric():
            return self.budgetAmount >= int(productCost)
        else:
            return False

    def FormatCurrencyToSymbol(self, amount, currencyCode) -> str:
        symbol = CURRENCY_SYMBOLS.get(currencyCode, currencyCode)

        return f"{symbol}{amount}"

    def HasBudgetBeenSet(self) -> bool:
        return self.hasBudgetBeenSet

class NetworkHandler():
    def GetUserIP() -> str:
        return requests.get("https://api.ipify.org", timeout=5).text.strip()

    def GetCountryCodeFromIP(ip: str) -> str:
        try:
            r = requests.get(f"https://ipapi.co/{ip}/json/", timeout=5)
            r.raise_for_status()
            data = r.json()

            return data.get("country_code")
        except requests.RequestException as exception:
            print(exception)

            return None

class JsonHandler():
    def SaveBudgetToJson(initialBudgetAmount: int, initialBudgetDeadlineDuration: int, endDate: datetime.datetime, cost: int, fileName="BudgetData.json"):
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        if os.path.exists(path):
            with open(path, "r") as saveData:
                budgetInfoDictionary = json.load(saveData)
        else:
            budgetInfoDictionary = {}

        budgetInfoDictionary["Year"] = endDate.year
        budgetInfoDictionary["Month"] = endDate.month
        budgetInfoDictionary["Remaining_Budget"] = cost

        if initialBudgetAmount is not None:
            budgetInfoDictionary["Initial_Budget_Amount"] = initialBudgetAmount

        if initialBudgetDeadlineDuration is not None:
            budgetInfoDictionary["Initial_Budget_Duration"] = initialBudgetDeadlineDuration

        with open(path, "w") as saveData:
            json.dump(budgetInfoDictionary, saveData)

    def LoadBudgetFromJson(fileName="BudgetData.json") -> tuple[datetime.datetime, int, int, int, bool]:
        """
        Returns:
            tuple[endDate, remainingBudget, intitialBudgetAmount, initialBudgetDuration, result]
        """

        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        try:
            with open(path, "r") as saveData:
                budgetInfoDictionary = json.load(saveData)

                remainingBudget = budgetInfoDictionary["Remaining_Budget"]
                budgetResetDatetime = datetime.datetime(budgetInfoDictionary["Year"], budgetInfoDictionary["Month"], 1, 0, 0, 0)

                intitialBudgetDuration = budgetInfoDictionary["Initial_Budget_Duration"]
                initialBudgetAmount = budgetInfoDictionary["Initial_Budget_Amount"]
        except Exception as exception:
            print("No save file found yet!")
            print(exception)

            return None, 0, 0, 0, False

        return budgetResetDatetime, remainingBudget, initialBudgetAmount, intitialBudgetDuration, True

    def SaveUserData(userIP: str, userCountryCode: str, fileName="UserData.json"):
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        with open(path, "w") as userData:
            userDataDictionary = {"User_IP": userIP, "User_Country_Code": userCountryCode}
            json.dump(userDataDictionary, userData)

    def LoadUserData(fileName="UserData.json") -> tuple[str, str, bool]:
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        userIP = None
        userCountryCode = None

        try:
            with open(path, "r") as userData:
                userDataDictionary = json.load(userData)

                userIP = userDataDictionary["User_IP"]
                userCountryCode = userDataDictionary["User_Country_Code"]

            return userIP, userCountryCode, True
        except:
            return None, None, False

    def SaveProductPurchaseHistoryDataToJson(productsDictionaryList: dict, fileName="ProductsHistory.json"):
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        with open(path, "w") as saveData:
            json.dump(productsDictionaryList, saveData)

            print("Saved history!")

    def LoadProductPurchaseHistoryDataListFromJson(fileName="ProductsHistory.json") -> list[Product]:
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        try:
            with open(path, "r") as saveData:
                productsDictionaryList = json.load(saveData)

                productsList = []
                for productItem in productsDictionaryList:
                    product = Product(productItem["name"], productItem["cost"], productItem["description"])
                    productsList.append(product)

                return productsList
        except:
            return None

    def DeleteProductPurchaseHistoryData(fileName="ProductsHistory.json") -> bool:
        path = os.path.join(App.get_running_app().user_data_dir, fileName)

        try:
            with open(path, "w") as saveData:
                json.dump("", saveData)

            return True
        except:
            return False

CURRENCY_SYMBOLS = {
    # Americas
    "USD": "$",    # United States
    "CAD": "$",    # Canada
    "MXN": "$",    # Mexico
    "BRL": "R$",   # Brazil
    "ARS": "$",    # Argentina
    "CLP": "$",    # Chile
    "COP": "$",    # Colombia
    "PEN": "S/",   # Peru

    # Europe
    "EUR": "€",    # Eurozone
    "GBP": "£",    # United Kingdom
    "CHF": "CHF",  # Switzerland
    "SEK": "kr",   # Sweden
    "NOK": "kr",   # Norway
    "DKK": "kr",   # Denmark
    "PLN": "zł",   # Poland
    "CZK": "Kč",   # Czech Republic
    "HUF": "Ft",   # Hungary
    "RON": "lei",  # Romania
    "BGN": "лв",   # Bulgaria
    "ISK": "kr",   # Iceland

    # Asia
    "JPY": "¥",    # Japan
    "CNY": "¥",    # China
    "KRW": "₩",    # South Korea
    "INR": "₹",    # India
    "IDR": "Rp",   # Indonesia
    "THB": "฿",    # Thailand
    "VND": "₫",    # Vietnam
    "PHP": "₱",    # Philippines
    "MYR": "RM",   # Malaysia
    "SGD": "$",    # Singapore
    "HKD": "$",    # Hong Kong
    "TWD": "NT$",  # Taiwan
    "PKR": "₨",    # Pakistan
    "BDT": "৳",    # Bangladesh
    "LKR": "Rs",   # Sri Lanka
    "NPR": "₨",    # Nepal

    # Middle East
    "ILS": "₪",    # Israel
    "SAR": "﷼",    # Saudi Arabia
    "AED": "د.إ",  # UAE
    "QAR": "﷼",    # Qatar
    "KWD": "د.ك",  # Kuwait
    "OMR": "﷼",    # Oman
    "JOD": "د.ا",  # Jordan
    "TRY": "₺",    # Turkey
    "IRR": "﷼",    # Iran

    # Africa
    "ZAR": "R",    # South Africa
    "NGN": "₦",    # Nigeria
    "EGP": "£",    # Egypt
    "KES": "KSh",  # Kenya
    "GHS": "₵",    # Ghana
    "MAD": "د.م.", # Morocco
    "TND": "د.ت",  # Tunisia
    "DZD": "د.ج",  # Algeria

    # Oceania
    "AUD": "$",    # Australia
    "NZD": "$",    # New Zealand
    "FJD": "$",    # Fiji

    # Others
    "RUB": "₽",    # Russia
    "UAH": "₴",    # Ukraine
    "BYN": "Br",   # Belarus
    "KZT": "₸",    # Kazakhstan
}

if __name__ == '__main__':
    BudgetTrackerApp().run()

# To do:
# Make the description text in a product info popup have a static position despite the size of the description

# To do when/before building the app:
# Build the app for android