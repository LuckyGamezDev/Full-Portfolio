using System;
using static Program;


class Program {

    public static Random random = new Random();

    public static bool shouldRun = true;

    public static int currentRoomIndex = 0;

    static void Main(string[] args) {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("Welcome to DungeonRooms!");
        Console.WriteLine();
        Console.ResetColor();

        while (shouldRun) {
            StartGame();

            // Code under start game gets triggered when start game calls return/break
            Console.WriteLine("Game over, play again (yes/no)");

            string input = Console.ReadLine();

            if (input.ToLower() == "yes") {
                Console.Clear();
                Console.WriteLine("Restarting game...");
                Console.WriteLine();
            } else if (input.ToLower() == "no"){
                Console.WriteLine("Thanks for playing!");

                shouldRun = false;
            } else {
                Console.WriteLine("Invalid input, assuming 'no'");

                shouldRun = false;
            }
        }
    }

    public static void StartGame() {
        List<Item> playerInventory = new List<Item>();
        int minPlayerDefaultHealth = 40;
        int maxPlayerDefaultHealth = 70;
        int defaultPlayerHealth = random.Next(minPlayerDefaultHealth, maxPlayerDefaultHealth + 1);
        Player player = new Player(2, playerInventory);

        Equipment playerRandomWeapon = (Equipment)random.Next(0, System.Enum.GetValues(typeof(Equipment)).Length - 1);
        player.ChangePlayerWeapon(playerRandomWeapon, false);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("You start with " + player.health + " health.");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Your weapon is " + player.currentPlayerWeapon);
        Console.WriteLine();
        Console.ResetColor();

        currentRoomIndex = 0;

        while (shouldRun) {
            Room generatedRoom;
            GenerateRoom(out generatedRoom);
            currentRoomIndex++;

            Console.WriteLine("Entering room " + currentRoomIndex + "...");
            Console.WriteLine();

            if (generatedRoom.hasEnemy) {
                Enemy enemyInRoom = new Enemy(generatedRoom.enemy.enemyType);
                HandleEnemyEncounter(player, enemyInRoom);
            } else if (generatedRoom.hasItem) {
                if (generatedRoom.itemInRoom != null) {
                    HandleItemEncounter(player, generatedRoom);
                } else {
                    Console.WriteLine("ERROR: the itemType is null!");
                }
            } else {
                HandleEmptyRoom(player, ref generatedRoom);
            }

            if (player.health <= 0) {
                break;
            }
        }
    }


    public static void GenerateRoom(out Room room) {
        string roomName;
        bool hasEnemy;
        bool hasItem;

        Random random = new Random();
        Rooms roomType = (Rooms)random.Next(0, System.Enum.GetValues(typeof(Rooms)).Length); // Casting the rooms so it can randomize

        roomName = roomType.ToString();

        int randomInt100 = random.Next(0, 101);

        // Check if the random int is even or not, if even set the bool to true
        if (randomInt100 % 2 == 0) {
            hasEnemy = true;

            int randomChance = random.Next(1, 3);

            if (randomChance % 2 == 0) { // If the random chance is even, make hasItem true, else false
                hasItem = true;
            } else {
                hasItem = false;
            }

        } else {
            hasEnemy = false;

            int randomChance = random.Next(1, 3);

            if (randomChance % 2 == 0) { // If the random chance is even, make hasItem true, else false
                hasItem = true;
            } else {
                hasItem = false;
            }
        }

        // Generate an enemy if the room has one
        Enemy randomEnemy = null;
        Enemies? enemyType;
        if (hasEnemy) {
            enemyType = (Enemies)random.Next(0, System.Enum.GetValues(typeof(Enemies)).Length);

            randomEnemy = new Enemy(enemyType.Value);
        }

        Item randomItem;
        if (hasItem) {
            Equipment itemType = (Equipment)random.Next(0, System.Enum.GetValues(typeof(Equipment)).Length);
            randomItem = new Item(itemType);
        } else {
            randomItem = null;
        }

        Room generatedRoom = new Room(roomName, hasEnemy, hasItem, randomEnemy, randomItem);

        room = generatedRoom;
    }

    public static void HandleEnemyEncounter(Player player, Enemy enemy) {
        bool hasRan = false;

        while (enemy.health > 0 && player.health > 0) { // Run this in a while to avoid stackoverflow (calling the function within the function)
            if (!hasRan) {
                Console.WriteLine("You see a " + enemy.enemyType + " smirling at you");
                Console.WriteLine();

                hasRan = true;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("What will you do:");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("1. " + "Attack");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("2. " + "Flee");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine(">");
            string input = Console.ReadLine();

            if (input != null) {
                if (input.ToLower() == "1") {
                    Item playerWeapon = new Item(player.currentPlayerWeapon);

                    int randomDamageAmount = random.Next(playerWeapon.minQuantity, playerWeapon.maxQuantity);
                    int randomSuccesRateAmount = random.Next(0, 101);

                    if (randomSuccesRateAmount <= playerWeapon.maxSuccesRate && randomSuccesRateAmount >= 1) {
                        Console.WriteLine("You strike the " + enemy.enemyType + " dealing " + randomDamageAmount + " damage");

                        enemy.health -= randomDamageAmount;

                        if (enemy.health <= 0) {
                            Console.WriteLine("The " + enemy.enemyType + " is now dead!");
                            break;
                        }
                    } else {
                        int randomEnemyDamageAmount = random.Next(enemy.minDealableDamage, enemy.maxDealableDamage + 1);

                        Console.WriteLine("You failed to attack the " + enemy.enemyType);
                        Console.WriteLine("Enemy dealt " + randomEnemyDamageAmount + " damage");
                        Console.WriteLine();

                        player.health -= randomEnemyDamageAmount;

                        if (player.health < 0) {
                            player.health = 0;
                        }

                        Console.WriteLine("New player health: " + player.health);

                        if (player.health <= 0) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("You died!");
                            Console.WriteLine();
                            Console.ResetColor();

                            return;
                        }
                    }
                } else {
                    int randomSuccesChance = random.Next(1, 3); // Make it a 50/50 chance to escape

                    if (randomSuccesChance % 2 == 0) { // Check if the random gened number is even, if so, let the player escape, else not
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("You succesfully escaped the enemy!");
                        Console.WriteLine();
                        Console.ResetColor();

                        break;
                    } else {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("You failed to escape the enemy");
                        Console.WriteLine();

                        int randomEnemyDamageAmount = random.Next(enemy.minDealableDamage, enemy.maxDealableDamage + 1);

                        player.health -= randomEnemyDamageAmount;

                        Console.WriteLine("The enemy hit you, losing " + randomEnemyDamageAmount + " health!");
                        Console.WriteLine();
                        Console.ResetColor();

                        if (player.health < 0) {
                            player.health = 0;
                        }

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("New player health: " + player.health);
                        Console.WriteLine();

                        if (player.health <= 0) {
                            if (player.health <= 0) {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("You died!");
                                Console.WriteLine();
                                Console.ResetColor();

                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void HandleItemEncounter(Player player, Room room) {
        if (room.itemInRoom.itemType != Equipment.healthPotion) {
            Console.WriteLine("There's a " + room.itemInRoom.itemType + " in the corner of the room, will you pick it up?");
            Console.WriteLine();
        } else {
            Console.WriteLine("There's a " + room.itemInRoom.itemType + " in the corner of the room, will you use it?");
            Console.WriteLine();
        }

        Console.WriteLine("1. " + "Yes");
        Console.WriteLine("2. " + "No");
        Console.WriteLine();

        string input = Console.ReadLine();

        if (input.ToLower() == "1" || input.ToLower() == "yes") {
            player.UseItem(room.itemInRoom);
        } else if (input.ToLower() == "2" || input.ToLower() == "no") {
            return;
        } else {
            Console.WriteLine("This is not an valid input, please try again");

            bool hasFilledInCorrectInput = false;
            while (!hasFilledInCorrectInput) {
                Console.WriteLine("1. " + "Yes");
                Console.WriteLine("2. " + "No");

                input = Console.ReadLine();

                if (input.ToLower() == "1" || input.ToLower() == "yes") {
                    hasFilledInCorrectInput = true;
                } else if (input.ToLower() == "2" || input.ToLower() == "no") {
                    hasFilledInCorrectInput = true;
                }
            }
        }
    }

    public static void HandleEmptyRoom(Player player, ref Room room) {
        int randomChance = random.Next(0, 3); // Make it a 33/33/33/ chance to assign something to the room to make empty less common

        if (randomChance == 1) { // If even, change has enemy
            room.hasEnemy = true;

            // Generate a random enemy type for the enemy in the room
            Enemies enemyType = (Enemies)random.Next(0, System.Enum.GetValues(typeof(Enemies)).Length);

            room.enemy = new Enemy(enemyType);

            HandleEnemyEncounter(player, room.enemy);
        } else if (randomChance == 2) { // if uneven with 1 left, change has item
            room.hasItem = true;

            // Generate a new random type of item for the room item
            Equipment itemType = (Equipment)random.Next(0, System.Enum.GetValues(typeof(Equipment)).Length);

            room.itemInRoom = new Item(itemType);

            HandleItemEncounter(player, room);

        } else {
            Console.WriteLine("There was nothing in this room, moving to next room.");
            Console.WriteLine();
        }
    }

    public class Player {
        public int health;
        public Equipment currentPlayerWeapon = default;

        public List<Item> consumableInventory = new List<Item>();

        public Player(int health, List<Item> inventory) {
            this.health = health;
            this.consumableInventory = inventory;
        }

        public void ChangePlayerWeapon(Equipment newWeapon, bool shouldLog = true) {
            currentPlayerWeapon = newWeapon;

            if (shouldLog) {
                Console.WriteLine("Changed weapon, " + currentPlayerWeapon + " is your new weapon");
            }
        }

        public void UseItem(Item item) {
            Random random = new Random();
            int randomAmount = random.Next(item.minQuantity, item.maxQuantity + 1);

            if (item.itemType == Equipment.healthPotion) {
                health += randomAmount;

                Console.WriteLine("New player health: " + health);
                Console.WriteLine();
            } else {
                ChangePlayerWeapon(item.itemType);

                Console.WriteLine("Picked up a " + currentPlayerWeapon + " , this is your new weapon");
                Console.WriteLine();
            }
        }

        public List<Item> GetPlayerConsumableInventory() {
            return consumableInventory;
        }
    }

    public class Enemy {
        public int health;
        public int minDealableDamage = 4;
        public int maxDealableDamage = 11;

        public Enemies enemyType;

        public Enemy(Enemies enemyType) {
            this.enemyType = enemyType;

            switch (enemyType) {
                case Enemies.Styx:
                    health = 12;
                    break;
                case Enemies.Alien:
                    health = 7;
                    break;
                case Enemies.Caveman:
                    health = 5;
                    break;
                case Enemies.Kangeroo:
                    health = 9;
                    break;
            }
        }
    }

    public class Item {
        public int maxQuantity;
        public int minQuantity;

        public int maxSuccesRate;

        public Equipment itemType;

        public Item(Equipment assignedItem) {
            this.itemType = assignedItem;

            switch (assignedItem) {
                case Equipment.None:
                    maxQuantity = 0;
                    minQuantity = 0;

                    maxSuccesRate = 20;
                    break;
                case Equipment.Dagger:
                    maxQuantity = 6;
                    minQuantity = 3;

                    maxSuccesRate = 65;
                    break;
                case Equipment.Spear:
                    maxQuantity = 4;
                    minQuantity = 1;

                    maxSuccesRate = 85;
                    break;
                case Equipment.RustySword:
                    maxQuantity = 4;
                    minQuantity = 2;

                    maxSuccesRate = 70;
                    break;
                case Equipment.Knife:
                    maxQuantity = 6;
                    minQuantity = 3;

                    maxSuccesRate = 60;
                    break;
                case Equipment.healthPotion:
                    maxQuantity = 15;
                    minQuantity = 8;

                    maxSuccesRate = 100;
                    break;
            }
        }
    }

    public class Room {
        public string roomName;
        public bool hasItem;

        public bool hasEnemy;
        public Enemy enemy;
        public Item? itemInRoom;

        public Room(string name, bool hasEnemy, bool hasItem, Enemy enemy = default, Item? itemType = default) {
            roomName = name;
            this.hasEnemy = hasEnemy;
            this.hasItem = hasItem;

            this.enemy = enemy;

            if (!hasItem) {
                this.itemInRoom = null;
            } else {
                this.itemInRoom = itemType;
            }
        }
    }

    public enum Rooms {
        Hall,
        Library,
        Dungeon,
        Attic,
    }

    public enum Enemies {
        Styx,
        Caveman,
        Alien,
        Kangeroo
    }

    // TODO: add more items
    public enum Equipment {
        None,
        RustySword,
        Knife,
        Dagger,
        Spear,
        healthPotion,
    }
}