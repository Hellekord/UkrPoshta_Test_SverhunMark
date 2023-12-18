using System.Data.SqlClient;
using System.Collections.Generic;
using UkrPoshta_Test.Models;
using System.Text;

namespace UkrPoshta_Test.Data
{
    public class EmployeeDataAccess : IEmployeeDataAccess // Этот класс предоставляет операции доступа к данным, связанным с сотрудниками.
    {
        private readonly DatabaseConnection _databaseConnection; // Используется для создания подключений к базе данных.
        private readonly ILogger _logger; //Используется для логирования

        public EmployeeDataAccess(DatabaseConnection databaseConnection, ILogger<EmployeeDataAccess> logger) //Сохраняем конект в частном поле, создаем логер для логирования ошибок
        {
            _databaseConnection = databaseConnection;
            _logger = logger;
        }


        public List<Employee> GetAllEmployees() //// Извлекает все записи о сотрудниках из БД
        {
            List<Employee> employees = new List<Employee>(); //Cписок для хранения полученных сотрудников.
            string sqlQuery = "SELECT * FROM Employee"; // SQL-запрос для выбора всех полей из таблицы Employee.


            using (var connection = _databaseConnection.CreateConnection()) //using для очистки
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open(); // Открытие соединения с базой данных.
                using (var reader = command.ExecuteReader()) // Выполняем команду и используем SqlDataReader для получения данных
                {
                    while (reader.Read()) // Читаем каждую строку 
                    {
                        employees.Add(new Employee // Создаем объект «Сотрудник» и заполняем его данными из текущей строки.
                        {
                            EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                            PositionID = reader.GetInt32(reader.GetOrdinal("PositionID")),
                            Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
                        });
                    }
                }
            }
            return employees; // Возвращаем список сотрудников.

        }

        public Employee GetEmployeeById(int id) //Получаем пользователя по id
        {
            Employee employee = null; 
            string sqlQuery = "SELECT * FROM Employee WHERE EmployeeID = @EmployeeID"; //Для выбора всех полей для одного сотрудника на основе id.

            try
            {
                using (var connection = _databaseConnection.CreateConnection()) // Создаем и открываем соединение с базой данных.
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", id); // Добавляем параметр «EmployeeID» в SQL-запрос.
                    connection.Open();
                    using (var reader = command.ExecuteReader()) // Выполняем команду и используем SqlDataReader для получения результата.
                    {
                        if (reader.Read()) // Если сотрудник с данным идентификатором существует, читаем его данные.
                        {
                            employee = new Employee // Заполняем объект сотрудника данными из базы данных.
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Telephone = reader.GetString(reader.GetOrdinal("Telephone")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                DateOfEmployment = reader.GetDateTime(reader.GetOrdinal("DateOfEmployment")),
                                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                                DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                                PositionID = reader.GetInt32(reader.GetOrdinal("PositionID"))
                            };
                        }
                    }
                }
            }
            catch (SqlException ex) 
            {
                _logger.LogError(ex, "Error occurred while getting employee with ID {EmployeeID}", id); // Регистрируем исключение, если возникла ошибка, связанная с SQL.
            }

            return employee; // Вернем объект, который будет иметь значение null, если сотрудник не найден.
        }

        public int? GetDepartmentIdByName(string departmentName) // Метод получения идентификатора отдела по его названию.
        {
            string sqlQuery = "SELECT DepartmentID FROM Department WHERE DepartmentName = @DepartmentName"; // SQL-запрос для выбора DepartmentID на основе DepartmentName.

            try
            {
                using (var connection = _databaseConnection.CreateConnection()) // Создаем и открываем соединение с базой данных.
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentName", departmentName); // Добавляем параметр DepartmentName в SQL-запрос.
                    connection.Open();
                    var result = command.ExecuteScalar(); // Выполняем запрос и сохраняем результат, если он есть.
                    return result != DBNull.Value ? (int?)result : null; // Возвращаем DepartmentID или ноль, если не нашли.
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error occurred while getting DepartmentID for DepartmentName: {DepartmentName}", departmentName); // Регистрируем исключение, если возникла ошибка, связанная с SQL.
                return null;
            }
        }

        public int? GetPositionIdByName(string positionName) // Метод получения id Position по ее названию
        {
            string sqlQuery = "SELECT PositionID FROM Position WHERE PositionName = @PositionName"; // SQL-запрос для выбора PositionID на основе PositionName.

            try
            {
                using (var connection = _databaseConnection.CreateConnection()) // Устанавливаем соединение с базой данных.
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@PositionName", positionName); // Добавляем параметр PositionName в команду SQL.
                    connection.Open();
                    var result = command.ExecuteScalar(); // Выполняем команду и сохраняем результат.
                    return result != DBNull.Value ? (int?)result : null; // Проверка на null
                }
            }
            catch (SqlException ex) // Регистрируем исключение в целях отладки.
            {
                _logger.LogError(ex, "Error occurred while getting PositionID for PositionName: {PositionName}", positionName);
                return null;
            }
        }


        public bool UpdateEmployeeDetails(int id, Employee employeeUpdate) // Метод обновления сведений о сотруднике через id
        {
            // SQL-запрос для обновления сведений о конкретном сотруднике.
            string sqlQuery = @" 
        UPDATE Employee
        SET FullName = @FullName, 
            DepartmentID = @DepartmentID, 
            PositionID = @PositionID,
            Salary = @Salary
        WHERE EmployeeID = @EmployeeID";

            try
            {
                using (var connection = _databaseConnection.CreateConnection()) // Открытие соединения с базой данных.
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", id); // Присвоение значений параметрам в команде SQL.
                    command.Parameters.AddWithValue("@FullName", employeeUpdate.FullName);
                    command.Parameters.AddWithValue("@DepartmentID", employeeUpdate.DepartmentID);
                    command.Parameters.AddWithValue("@PositionID", employeeUpdate.PositionID);
                    command.Parameters.AddWithValue("@Salary", employeeUpdate.Salary);

                    connection.Open();
                    int result = command.ExecuteNonQuery(); 
                    return result > 0; // Выполняем команду и возвращаем true, если хотя бы одна запись была обновлена.
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the employee details with ID {EmployeeID}", id);
                return false; // Возвращаем false, чтобы указать, что обновление не удалось
            }
        }



        public IEnumerable<Department> GetAllDepartments() // Извлекает все Department из базы данных.
        {
            var departments = new List<Department>();
            string sqlQuery = "SELECT DepartmentID, DepartmentName FROM Department"; // SQL-запрос для выбора всех id и названий отделов.

            using (var connection = _databaseConnection.CreateConnection()) // Установление соединения и команда для выполнения SQL-запроса.
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                connection.Open(); // Открытие соединения с базой
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // Чтение всех возвращенных строк отдела.
                    {
                        departments.Add(new Department // Добавляем в лист каждый отдел с его id.
                        {
                            DepartmentID = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName"))
                        });
                    }
                }
            }
            return departments; // Возвращаем лист отделов.
        }

        public IEnumerable<string> GetSalaryReportByDepartment(string departmentName) // Генерирует отчет о зарплате для данного отдела по названию.
        {
            var reportData = new List<string>();
            // SQL-запрос для выбора полных имен и зарплат сотрудников, если название отдела совпадает.
            string sqlQuery = @" 
        SELECT e.FullName, e.Salary, d.DepartmentName
        FROM Employee e
        INNER JOIN Department d ON e.DepartmentID = d.DepartmentID
        WHERE d.DepartmentName = @DepartmentName"; 
             
            using (var connection = _databaseConnection.CreateConnection()) // Создание и выполнение команды SQL.
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@DepartmentName", departmentName); // Добавление названия отдела в качестве параметра
                connection.Open(); // Открытие соединения.
                using (var reader = command.ExecuteReader()) // Чтение каждого сотрудника отдела.
                {
                    while (reader.Read())
                    {
                        var fullName = reader.GetString(reader.GetOrdinal("FullName"));
                        var salary = reader.GetDecimal(reader.GetOrdinal("Salary"));
                        reportData.Add($"{fullName}: {salary}"); // Добавляем имя и зарплату каждого сотрудника в список данных отчета.
                    }
                }
            }
            return reportData; // Возвращаем данные отчета о зарплате.
        }

        public IEnumerable<string> ExportSalaryReport(string departmentName) // Экспортирует отчет о зарплате для данного отдела в формат, подходящий для текстового файла.
        {
            int? departmentId = GetDepartmentIdByName(departmentName); // Сначала id отдела по названию.
            if (!departmentId.HasValue)
            {
                return null; // Если идентификатор отдела не найден, возвращаем null.
            }

            var reportData = new List<string>();
            // SQL-запрос для выбора имен и зарплат сотрудников для определенного id.
            string sqlQuery = @"
    SELECT e.FullName, e.Salary
    FROM Employee e
    WHERE e.DepartmentID = @DepartmentID";

            using (var connection = _databaseConnection.CreateConnection()) // Установление соединения с базой данных и команда.
            using (var command = new SqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@DepartmentID", departmentId.Value); // Добавляем id в качестве параметра.
                connection.Open(); // Открытие соединения.
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // Чтение каждого сотрудника и добавление его имени и зарплаты в данные отчета.
                    {
                        var fullName = reader.GetString(reader.GetOrdinal("FullName"));
                        var salary = reader.GetDecimal(reader.GetOrdinal("Salary")).ToString("C");
                        reportData.Add($"{fullName}: {salary}");
                    }
                }
            }

            return reportData; // Возвращаем отформатированные данные отчета о зарплате.
        }

        public string GetDepartmentNameById(int departmentId) // Получаем название отдела по id.
        {
            string departmentName = string.Empty;
            string sqlQuery = "SELECT DepartmentName FROM Department WHERE DepartmentID = @DepartmentID"; // SQL-запрос для выбора названия отдела для данного ud.

            try
            {
                using (var connection = _databaseConnection.CreateConnection())
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", departmentId); // Добавление id в параметры команды
                    connection.Open();
                    var result = command.ExecuteScalar(); // Выполняет команду и возвращает первый столбец первой строки.
                    if (result != null) // Преобразуем результат в строку, если он не равен нулю.
                    {
                        departmentName = result.ToString();
                    }
                }
            }
            catch (SqlException ex)// Зарегистрируем исключение если возникла проблема связанная с SQL.
            {
                _logger.LogError(ex, "Error occurred while getting department name for DepartmentID: {DepartmentID}", departmentId);
            }

            return departmentName; // Возвращаем полученное название отдела.
        }
    


    public CompanyInfo GetCompanyInfo() // Извлекает информацию о компании из базы данных.
        {
            CompanyInfo companyInfo = null;
            string sqlQuery = "SELECT TOP 1 CompanyInfoID, CompanyDetails FROM CompanyInfo"; // SQL-запрос для выбора первой записи компании.

            try
            {
                using (var connection = _databaseConnection.CreateConnection())
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            companyInfo = new CompanyInfo // Заполняет объект CompanyInfo данными
                            {
                                CompanyInfoID = reader.GetInt32(reader.GetOrdinal("CompanyInfoID")), 
                                CompanyDetails = reader.GetString(reader.GetOrdinal("CompanyDetails")),
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Під час отримання інформації про компанію сталася помилка."); //Логирование
                throw;
            }

            return companyInfo; // Возвращаем полученную информацию о компании.
        }

        public IEnumerable<string> ExportEmployeesData() // Экспортирует все данные о сотрудниках в список строк, подходящий для текстового файла.
        {
            var employeeData = new List<string>();
            string sqlQuery = "SELECT * FROM Employee"; // SQL-запрос для выбора всех полей из таблицы Employee

            try
            {
                using (var connection = _databaseConnection.CreateConnection())
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Форматирует данные в строку CSV для каждого сотрудника.
                            var employeeInfo = $"{reader["EmployeeID"]},{reader["FullName"]},{reader["Address"]},{reader["Telephone"]},{reader["DateOfBirth"]},{reader["DateOfEmployment"]},{reader["Salary"]},{reader["DepartmentID"]},{reader["PositionID"]}";
                            employeeData.Add(employeeInfo); // Добавляем строку CSV в лист.
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Під час експорту даних про співробітника сталася помилка.");
                throw;
            }

            return employeeData; // Возвращаем список строк данных о сотрудниках.
        }


        public List<Employee> SearchEmployees(string fullName, string departmentName, string positionName) // Ищем сотрудников по предоставленному полному имени, названию отдела и названию должности.
        {
            List<Employee> employees = new List<Employee>();
            // Динамическое построение SQL-запроса на основе критериев поиска.
            StringBuilder sqlQueryBuilder = new StringBuilder(@"
        SELECT e.EmployeeID, e.FullName, e.Salary, d.DepartmentName, p.PositionName 
        FROM Employee e
        JOIN Department d ON e.DepartmentID = d.DepartmentID
        JOIN Position p ON e.PositionID = p.PositionID
        WHERE 1=1"); // WHERE 1=1 используется как отправная точка для добавления дополнительных условий.

            // Добавляем условия в SQL-запрос на основе непустого критерия поиска.
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                sqlQueryBuilder.Append(" AND e.FullName LIKE @FullName");
            }
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                sqlQueryBuilder.Append(" AND d.DepartmentName LIKE @DepartmentName");
            }
            if (!string.IsNullOrWhiteSpace(positionName))
            {
                sqlQueryBuilder.Append(" AND p.PositionName LIKE @PositionName");
            }

            string sqlQuery = sqlQueryBuilder.ToString(); // Преобразуем Builder в строку.

            try
            {
                using (var connection = _databaseConnection.CreateConnection())
                using (var command = new SqlCommand(sqlQuery, connection))
                {
                    // Добавляем параметры в команду
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        command.Parameters.AddWithValue("@FullName", $"%{fullName}%");
                    }
                    if (!string.IsNullOrWhiteSpace(departmentName))
                    {
                        command.Parameters.AddWithValue("@DepartmentName", $"%{departmentName}%");
                    }
                    if (!string.IsNullOrWhiteSpace(positionName))
                    {
                        command.Parameters.AddWithValue("@PositionName", $"%{positionName}%");
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Читаем каждую строку и создаем объект Employee.
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                PositionName = reader.GetString(reader.GetOrdinal("PositionName")),
                                //Если нужно то можем инициализировать другие свойства по мере необходимости.
                            });
                        }
                    }
                }
            }
            catch (SqlException ex) //Регестрируем исключение
            {
                _logger.LogError(ex, "An error occurred while searching for employees.");
                throw;
            }

            return employees;
        }
    }
}

