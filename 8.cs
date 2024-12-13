using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EnterpriseSystem
{
    class Program
    {
        static string UsersFilePath = "users.json";
        static string ItemsFilePath = "items.json";
        static List<User> Users = new List<User>();
        static List<Item> Items = new List<Item>();
        static List<CartItem> Cart = new List<CartItem>();

        static void Main(string[] args)
        {
            LoadData();

            if (!Users.Any(u => u.Role == "Администратор"))
            {
                CreateDefaultAdmin();
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Система управления предприятием ===");
                Console.WriteLine("1. Войти");
                Console.WriteLine("2. Выйти");
                Console.Write("Выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        SaveData();
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        #region Авторизация и создание администратора
        static void Login()
        {
            Console.Clear();
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            string password = HidePassword();

            User user = Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                Console.WriteLine($"\nДобро пожаловать, {user.Role}!");
                NavigateRole(user);
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
                Console.ReadKey();
            }
        }

        static string HidePassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        static void CreateDefaultAdmin()
        {
            Console.WriteLine("Создание администратора...");
            Users.Add(new User
            {
                Login = "admin",
                Password = "Admin123**",
                Role = "Администратор",
                FIO = "Системный Администратор",
                BirthDate = new DateTime(1980, 1, 1),
                Education = "Высшее",
                Experience = 10,
                Position = "Администратор",
                Salary = 0
            });
            SaveData();
            Console.WriteLine("Администратор создан. Перезапустите программу.");
            Environment.Exit(0);
        }
        #endregion

        #region Меню и функционал ролей
        static void NavigateRole(User user)
        {
            switch (user.Role)
            {
                case "Администратор":
                    AdminMenu();
                    break;
                case "Кадры":
                    PersonnelMenu();
                    break;
                case "Склад":
                    WarehouseMenu();
                    break;
                case "Касса-продавец":
                    SellerMenu();
                    break;
                case "Бухгалтерия":
                    FinanceMenu();
                    break;
                case "Покупатель":
                    CustomerMenu(user);
                    break;
                default:
                    Console.WriteLine("Неизвестная роль.");
                    break;
            }
        }

        #region Администратор
        static void AdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню администратора ===");
                Console.WriteLine("1. Добавить пользователя");
                Console.WriteLine("2. Удалить пользователя");
                Console.WriteLine("3. Редактировать пользователя");
                Console.WriteLine("4. Просмотреть всех пользователей");
                Console.WriteLine("5. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddUser();
                        break;
                    case "2":
                        DeleteUser();
                        break;
                    case "3":
                        EditUser();
                        break;
                    case "4":
                        ViewUsers();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddUser()
        {
            Console.Clear();
            Console.WriteLine("=== Добавление пользователя ===");
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(login) || Users.Any(u => u.Login == login))
            {
                Console.WriteLine("Некорректный или уже существующий логин.");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите пароль: ");
            string password = Console.ReadLine();


            Console.Write("Введите роль (Администратор, Кадры, Склад, Касса-продавец, Бухгалтерия, Покупатель): ");
            string role = Console.ReadLine();

            Users.Add(new User
            {
                Login = login,
                Password = password,
                Role = role
            });

            SaveData();
            Console.WriteLine("Пользователь добавлен!");
            Console.ReadKey();
        }

        static void DeleteUser()
        {
            Console.Clear();
            Console.WriteLine("=== Удаление пользователя ===");
            Console.Write("Введите логин пользователя для удаления: ");
            string login = Console.ReadLine();

            User user = Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                Users.Remove(user);
                SaveData();
                Console.WriteLine("Пользователь удалён.");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }

            Console.ReadKey();
        }

        static void EditUser()
        {
            Console.Clear();
            Console.WriteLine("=== Редактирование пользователя ===");
            Console.Write("Введите логин пользователя: ");
            string login = Console.ReadLine();

            User user = Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                Console.Write("Введите новое ФИО: ");
                user.FIO = Console.ReadLine();

                Console.Write("Введите новую должность: ");
                user.Position = Console.ReadLine();

                SaveData();
                Console.WriteLine("Пользователь обновлён.");
            }
            else
            {
                Console.WriteLine("Пользователь не найден.");
            }

            Console.ReadKey();
        }

        static void ViewUsers()
        {
            Console.Clear();
            Console.WriteLine("=== Список пользователей ===");
            foreach (var user in Users)
            {
                Console.WriteLine($"Логин: {user.Login}, Роль: {user.Role}, ФИО: {user.FIO}");
            }
            Console.ReadKey();
        }

        static bool ValidatePassword(string password)
        {
            if (password.Length < 8)
                return false;
            if (password.Count(char.IsUpper) < 3)
                return false;
            if (password.Count(char.IsDigit) < 3)
                return false;
            if (password.Count(c => "!@#$%^&*()_+-=[]{}".Contains(c)) < 2)
                return false;

            return true;
        }
        #endregion

        #region Кадры
        static void PersonnelMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню отдела кадров ===");
                Console.WriteLine("1. Просмотреть список сотрудников");
                Console.WriteLine("2. Добавить нового сотрудника");
                Console.WriteLine("3. Уволить сотрудника");
                Console.WriteLine("4. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewUsers();
                        break;
                    case "2":
                        AddUser();
                        break;
                    case "3":
                        DeleteUser();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }
        #endregion

        #region Склад
        static void WarehouseMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню склада ===");
                Console.WriteLine("1. Просмотреть список товаров");
                Console.WriteLine("2. Добавить новый товар");
                Console.WriteLine("3. Списать товар");
                Console.WriteLine("4. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewItems();
                        break;
                    case "2":
                        AddItem();
                        break;
                    case "3":
                        RemoveItem();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ViewItems()
        {
            Console.Clear();
            Console.WriteLine("=== Список товаров ===");
            foreach (var item in Items)
            {
                Console.WriteLine($"Наименование: {item.Name}, Количество: {item.Quantity}, Цена: {item.Price}");
            }
            Console.ReadKey();
        }

        static void AddItem()
        {
            Console.Clear();
            Console.WriteLine("=== Добавление товара ===");
            Console.Write("Введите название товара: ");
            string name = Console.ReadLine();

            Console.Write("Введите количество товара: ");
            int quantity = int.Parse(Console.ReadLine());

            Console.Write("Введите цену товара: ");
            decimal price = decimal.Parse(Console.ReadLine());

            Items.Add(new Item { Name = name, Quantity = quantity, Price = price });
            SaveData();
            Console.WriteLine("Товар добавлен!");
            Console.ReadKey();
        }

        static void RemoveItem()
        {
            Console.Clear();
            Console.WriteLine("=== Списание товара ===");
            Console.Write("Введите название товара для списания: ");
            string name = Console.ReadLine();

            Item item = Items.FirstOrDefault(i => i.Name == name);
            if (item != null)
            {
                Items.Remove(item);
                SaveData();
                Console.WriteLine("Товар списан.");
            }
            else
            {
                Console.WriteLine("Товар не найден.");
            }

            Console.ReadKey();
        }
        #endregion

        #region Касса-продавец
        static void SellerMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню кассира-продавца ===");
                Console.WriteLine("1. Просмотреть доступные товары");
                Console.WriteLine("2. Оформить продажу");
                Console.WriteLine("3. Отменить продажу");
                Console.WriteLine("4. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewItems();
                        break;
                    case "2":
                        ProcessSale();
                        break;
                    case "3":
                        CancelSale();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ProcessSale()
        {
            Console.WriteLine("=== Оформление продажи ===");
            // Имитация оформления продажи
            Console.WriteLine("Продажа успешно оформлена.");
            Console.ReadKey();
        }

        static void CancelSale()
        {
            Console.WriteLine("=== Отмена продажи ===");
            // Имитация отмены продажи
            Console.WriteLine("Продажа отменена.");
            Console.ReadKey();
        }
        #endregion

        #region Бухгалтерия
        static void FinanceMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню бухгалтерии ===");
                Console.WriteLine("1. Просмотреть зарплаты сотрудников");
                Console.WriteLine("2. Просмотреть доходы компании");
                Console.WriteLine("3. Расчет налогов");
                Console.WriteLine("4. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewSalaries();
                        break;
                    case "2":
                        ViewCompanyIncome();
                        break;
                    case "3":
                        CalculateTaxes();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ViewSalaries()
        {
            Console.Clear();
            Console.WriteLine("=== Зарплаты сотрудников ===");
            foreach (var user in Users.Where(u => u.Role != "Администратор"))
            {
                Console.WriteLine($"{user.FIO} ({user.Position}): {user.Salary} руб.");
            }
            Console.ReadKey();
        }

        static void ViewCompanyIncome()
        {
            Console.WriteLine("=== Доходы компании ===");
            Console.WriteLine("Доходы компании: 1000000 руб."); // Пример данных
            Console.ReadKey();
        }

        static void CalculateTaxes()
        {
            Console.WriteLine("=== Расчет налогов ===");
            Console.WriteLine("Налоги составляют 20% от дохода компании.");
            Console.WriteLine("Налоги: 200000 руб."); // Пример данных
            Console.ReadKey();
        }
        #endregion

        #region Покупатель
        static void CustomerMenu(User user)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Меню покупателя ({user.FIO}) ===");
                Console.WriteLine("1. Просмотреть доступные товары");
                Console.WriteLine("2. Добавить товар в корзину");
                Console.WriteLine("3. Оформить заказ");
                Console.WriteLine("4. Назад");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewItems();
                        break;
                    case "2":
                        AddToCart(user);
                        break;
                    case "3":
                        Checkout(user);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddToCart(User user)
        {
            Console.Write("Введите название товара для добавления в корзину: ");
            string itemName = Console.ReadLine();
            Item item = Items.FirstOrDefault(i => i.Name == itemName);

            if (item != null)
            {
                Cart.Add(new CartItem { Item = item, User = user });
                Console.WriteLine("Товар добавлен в корзину.");
            }
            else
            {
                Console.WriteLine("Товар не найден.");
            }

            Console.ReadKey();
        }

        static void Checkout(User user)
        {
            Console.WriteLine("=== Оформление заказа ===");
            decimal totalPrice = 0;

            foreach (var cartItem in Cart.Where(c => c.User == user))
            {
                totalPrice += cartItem.Item.Price;
            }

            Console.WriteLine($"Итого к оплате: {totalPrice} руб.");
            Console.WriteLine("Спасибо за покупку!");
            Cart.RemoveAll(c => c.User == user); // Очищаем корзину после оформления заказа
            Console.ReadKey();
        }
        #endregion

        #region Сохранение и загрузка данных
        static void SaveData()
        {
            // Сериализация данных в JSON
            File.WriteAllText(UsersFilePath, JsonSerializer.Serialize(Users));
            File.WriteAllText(ItemsFilePath, JsonSerializer.Serialize(Items));
        }

        static void LoadData()
        {
            // Загрузка данных из JSON
            if (File.Exists(UsersFilePath))
            {
                Users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(UsersFilePath));
            }

            if (File.Exists(ItemsFilePath))
            {
                Items = JsonSerializer.Deserialize<List<Item>>(File.ReadAllText(ItemsFilePath));
            }
        }
        #endregion
    }

    #region Классы данных
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FIO { get; set; }
        public DateTime BirthDate { get; set; }
        public string Education { get; set; }
        public int Experience { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CartItem
    {
        public Item Item { get; set; }
        public User User { get; set; }
    }
}
#endregion
#endregion