import TimelyBudgetTracker

import datetime

from kivy.uix.boxlayout import BoxLayout
from kivy.uix.floatlayout import FloatLayout

from kivy.uix.widget import Widget
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.uix.popup import Popup

from kivy.clock import Clock

from kivy.properties import ObjectProperty

class SetBudgetPopup(Popup):
    dateSetterTextInput = ObjectProperty(None)

    def __init__(self, **kwargs):
        super().__init__(**kwargs)

        self.app = TimelyBudgetTracker.App.get_running_app()

    def OnNewBudgetSet(self, sender, budgetAmount: str, monthsAmount: str):
        if budgetAmount != "" and monthsAmount != "":
            if monthsAmount.isnumeric():
                monthsAmount = int(monthsAmount)
            else:
                return

            if budgetAmount.isnumeric():
                budgetAmount = int(budgetAmount)
            else:
                return

            self.app.SetNewBudget(budgetAmount, monthsAmount)

            sender.dismiss()

    def CalculateEndDate(monthsAmount: int) -> tuple[int, int]:
        currentMonth = datetime.datetime.now().month
        currentYear = datetime.datetime.now().year

        budgetResetMonth = ((currentMonth - 1 + monthsAmount) % 12) + 1 # currentMonth - 1 to account with the internal index (0-11 for month index)

        yearLoops = (currentMonth - 1 + monthsAmount) // 12
        budgetResetYear = currentYear + yearLoops

        return budgetResetYear, budgetResetMonth

class BudgetSpender(FloatLayout):
    def __init__(self, **kwargs):
        super().__init__(**kwargs)

        Clock.schedule_once(self.DelayedInit, 0)

    def DelayedInit(self, deltaTime):
        self.app = TimelyBudgetTracker.App.get_running_app()

        _, budgetAmount, _, _, result = TimelyBudgetTracker.JsonHandler.LoadBudgetFromJson()

        if result is not False:
            self.app.budgetAmount = budgetAmount

    def CreateProduct(self, sender, productInfoTuple: tuple):
        if productInfoTuple[0] != "" and productInfoTuple[1] != "":
            sender.dismiss()

            product = TimelyBudgetTracker.Product(*productInfoTuple)

            if self.app.CanAddProduct(product.cost):
                self.app.AddProduct(product)
            else:
                if self.app.budgetAmount > 0:
                    self.app.AddProduct(product)

    def CanAddProduct(self) -> bool:
        return self.app.budgetAmount > 0

class ProductPurchaseHistoryButton(Button):
    productName = None
    productCost = None
    productDescription = None

    def __init__(self, productName, productCost, productDescription, **kwargs):
        super().__init__(**kwargs)

        self.app = TimelyBudgetTracker.App.get_running_app()

        content = BoxLayout(orientation="horizontal", padding=0, size_hint=(1, 1))

        self.bind(size=content.setter("size"), pos=content.setter("pos"))

        content.add_widget(Label(
            text=productName,
            halign="left",
            valign="middle"
        ))

        content.add_widget(Label(
            text=self.app.FormatCurrencyToSymbol(productCost, self.app.userCurrency),
            halign="right",
            valign="middle"
        ))

        self.add_widget(content)

        self.productName = productName
        self.productCost = productCost
        self.productDescription = productDescription

class ProductPurchaseHistoryPurchaseInfoPopup(Popup):
    purchaseInfoFloatLayout = ObjectProperty(None)

    productToDisplay = ObjectProperty(None)

    def __init__(self, productToDisplay, **kwargs):
        super().__init__(**kwargs)

        self.productToDisplay = productToDisplay

class ProductPurchaseHistoryWidget(FloatLayout):
    productButtonsInHistoryList: list[ProductPurchaseHistoryButton] = []
    productsGrid = None

    noProductsLabel = None # Label which is shown when no history is yet available

    def UpdateFromApp(self, sender):
        self.app = TimelyBudgetTracker.App.get_running_app()

        self.productsGrid = sender.ids.history_scroll_view.children[0]

        self.UpdateHistory()

    def UpdateHistory(self):
        productsList = self.app.history.GetProductsList()

        if productsList == []:
            print("Nothing to update, no products yet added to history!")

            # Add a label which says "No purchases yet!"
            self.noProductsLabel = Label(text="No purchases yet logged!", bold=True, 
                pos_hint={"center_x": 0.5, "center_y": 0.6}, size_hint=(None, None), 
                halign="center", valign="middle"
            )

            if len(self.productsGrid.children) > 0:
                for button in reversed(self.productsGrid.children):
                    self.productsGrid.remove_widget(button)
                    self.productButtonsInHistoryList.remove(button)
            
            self.add_widget(self.noProductsLabel)

            return
        else: # Remove the no history label when there is history
            if self.noProductsLabel is not None:
                self.remove_widget(self.noProductsLabel)
                self.noProductsLabel = None

        if len(self.productsGrid.children) > 0:
            for button in reversed(self.productsGrid.children):
                self.productsGrid.remove_widget(button)
                self.productButtonsInHistoryList.remove(button)

        for productToAdd in productsList:
            productInfoButton = ProductPurchaseHistoryButton(productToAdd.name, productToAdd.cost, productToAdd.description, size_hint_y=None, height=140,
                on_release=self.OnButtonPurchaseInfoPressed, background_normal="", background_color=(0.15, 0.15, 0.15, 1)
            )

            self.productsGrid.add_widget(productInfoButton)
            self.productButtonsInHistoryList.append(productInfoButton)

    def OnButtonPurchaseInfoPressed(self, button):
        product = TimelyBudgetTracker.Product(button.productName, button.productCost, button.productDescription)

        popup = ProductPurchaseHistoryPurchaseInfoPopup(product)
        popup.open()

class BudgetTrackerTimer(Widget):
    countdownLabel = ObjectProperty(None)
    countdownEndDate = None

    def __init__(self, **kwargs):
        super().__init__(**kwargs)

        Clock.schedule_once(self.DelayedInit, 0)

    def DelayedInit(self, deltaTime):
        self.app = TimelyBudgetTracker.App.get_running_app()

        self.LoadBudgetMonthDuration()

        Clock.schedule_interval(self.UpdateCountdown, 1.0)

    def UpdateCountdown(self, deltatime):

        if not self.app.budgetEndDate:
            return

        days, hours, minutes, seconds = self.TimeUntil(self.app.budgetEndDate)

        if days <= 0:
            days = 0; hours = 0; minutes = 0; seconds = 0;

            self.ResetBudget()

        self.countdownLabel.text = f"Time until budget reset: {days}:{hours}:{minutes}:{seconds}"

    def TimeUntil(self, target_dateTime):
        now = datetime.datetime.now()
        delta = target_dateTime - now
        days, seconds = delta.days, delta.seconds
        hours = seconds // 3600
        minutes = (seconds % 3600) // 60
        seconds = seconds % 60

        return days, hours, minutes, seconds

    def LoadBudgetMonthDuration(self):
        self.app = TimelyBudgetTracker.App.get_running_app()
        self.countdownEndDate, _, _, _, result = TimelyBudgetTracker.JsonHandler.LoadBudgetFromJson()

        if result is not False:
            self.app.budgetEndDate = self.countdownEndDate
        else:
            print("No budget end date saved yet!")

    def ResetBudget(self):
        self.app.SetNewBudget(self.app.initialBudgetAmount, self.app.initialBudgetDuration)

        self.LoadBudgetMonthDuration()


class BudgetTrackerBudget(Widget):
    budgetAmountLabel = ObjectProperty(None)

    def __init__(self, **kwargs):
        super().__init__(**kwargs)

        Clock.schedule_once(self.DelayedInit, 0)

    def DelayedInit(self, deltatime):
        self.app = TimelyBudgetTracker.App.get_running_app()

        remainingUserBudget = self.app.budgetAmount
        self.budgetAmountLabel.text = f"Remaining Budget: {self.app.FormatCurrencyToSymbol(remainingUserBudget, self.app.userCurrency)}"
        
        initialUserBudget = self.app.initialBudgetAmount
        
        ratio = 0
        if remainingUserBudget > 0 and initialUserBudget > 0:
            ratio = remainingUserBudget / initialUserBudget

        if ratio == 1:
            self.budgetAmountLabel.color = (0, 1, 0, 1) # Bright green
        elif ratio <= 0.75 and ratio > 0.5:
            self.budgetAmountLabel.color = (0.5, 1, 0, 1) # Green-orange
        elif ratio <= 0.5 and ratio > 0.25:
            self.budgetAmountLabel.color = (1, 0.65, 0, 1) # Orange
        elif ratio <= 0.25 and ratio > 0:
            self.budgetAmountLabel.color = (1, 0, 0, 1) # Bright red
        elif ratio <= 0:
            self.budgetAmountLabel.color = (0.7, 0, 0.1, 1) # Dark red

        self.app.bind(budgetAmount=self.UpdateBudgetDisplay)

    def UpdateBudgetDisplay(self, instance, value):
        self.budgetAmountLabel.text = f"Remaining Budget: {self.app.FormatCurrencyToSymbol(value, self.app.userCurrency)}"

        initialUserBudget = self.app.initialBudgetAmount

        ratio = value / initialUserBudget

        if ratio == 1:
            self.budgetAmountLabel.color = (0, 1, 0, 1) # Bright green
        elif ratio <= 0.75 and ratio > 0.5:
            self.budgetAmountLabel.color = (0.5, 1, 0, 1) # Green-orange
        elif ratio <= 0.5 and ratio > 0.25:
            self.budgetAmountLabel.color = (1, 0.65, 0, 1) # Orange
        elif ratio <= 0.25 and ratio > 0:
            self.budgetAmountLabel.color = (1, 0, 0, 1) # Bright red
        elif ratio <= 0:
            self.budgetAmountLabel.color = (0.7, 0, 0.1, 1) # Dark red