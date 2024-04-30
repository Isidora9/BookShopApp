# BookShopApp

This web application was built using ASP.NET Core, Entity Framework, and SQL Server, allowing users to browse books, filter them, add books to a shopping cart, and make orders.

## Features
- **Browse books:** Users can view a list of available books, as well as best-selling and top-rated books.
- **Filter books:** Users can filter books based on different criteria, such as genre and author.
- **Add to cart:** Users can add books to their shopping cart.
- **Make an order:** Users can make orders for the books in their shopping cart.
- **User identity and authentication:** Authentication and authorization are implemented using ASP.NET Core Identity. Users can register, log in, and log out. Certain actions are restricted to authenticated users.
- **User roles:** Different roles (e.g., Admin, User) are implemented to control access to specific features or functionalities.
- **Unit tests:** Unit tests are implemented using xUnit to ensure the reliability of the application's functionalities.

## Technologies Used
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server
- Razor Pages
- xUnit
- HTML/CSS
- JavaScript

## Getting Started
### Prerequisites
- .NET Core SDK
- SQL Server

### Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/Isidora9/BookShopApp.git
    ```
2. Navigate to the project directory:
    ```bash
    cd BookShopApp
    ```
3. Update the connection string in `appsettings.json` with your SQL Server connection string:
    ```json
    "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=book_shop_db;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```
4. Apply database migrations:
    ```bash
    dotnet ef database update
    ```
5. Run the application:
    ```bash
    dotnet run
    ```
6. Open your browser and navigate to https://localhost.

## Running the Tests
Unit tests are implemented using xUnit. To run the tests, execute the following command in the terminal:
```bash
dotnet test
```

ASP.NET Core Version: 8.0