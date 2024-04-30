# BookShopApp

Welcome to the Bookshop App! This is a web application built using ASP.NET Core, Entity Framework, and SQL Server, allowing users to browse books, filter them, add books to a shopping cart, and make orders.

## Features
- **Browse books:** Users can view a list of available books.
- **Filter books:** Users can filter books based on different criteria such as category, author, etc.
- **Add to cart:** Users can add books to their shopping cart.
- **Make an order:** Users can make orders for the books in their shopping cart.
- **User identity and authentication:** Authentication and authorization are implemented using ASP.NET Core Identity. Users can register, log in, and log out. Certain actions are restricted to authenticated users.
- **User roles:** Different roles (e.g., Admin, Customer) are implemented to control access to specific features or functionalities.
- **Unit tests:** Unit tests are implemented using xUnit to ensure the correctness of the application's functionalities.

## Technologies Used
- ASP.NET Core MVC
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server
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
    git clone https://github.com/your-username/bookshop-app.git
    ```
2. Navigate to the project directory:
    ```bash
    cd bookshop-app
    ```
3. Update the connection string in `appsettings.json` with your SQL Server connection string:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=BookshopDb;Trusted_Connection=True;"
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
6. Open your browser and navigate to https://localhost:5001.

## Running the Tests
Unit tests are implemented using xUnit. To run the tests, execute the following command in the terminal:
```bash
dotnet test

ASP.NET Core Version: 8.0