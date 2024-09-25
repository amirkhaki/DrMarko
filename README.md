# DrMarko - ASP.NET Core MVC Backend for Aviato Bootstrap

DrMarko is an **ASP.NET Core MVC** backend project designed to work with the [Aviato Bootstrap](https://github.com/themefisher/aviato-bootstrap) theme. It serves dynamic content and manages server-side rendering for the Aviato Bootstrap frontend.

## Project Overview

This backend is built using **ASP.NET Core MVC** and provides a seamless integration with the Aviato Bootstrap template. It enables dynamic content management, user interactions, and other backend services for the Aviato theme.

### Features

- **ASP.NET Core MVC**: Handles routing, views, and server-side logic.
- **Entity Framework Core**: Manages data persistence and database interactions.
- **Razor Views**: Utilizes Razor for rendering dynamic HTML content.
- **Dependency Injection**: Implements DI for easy service management.
- **Authentication and Authorization**: Handles user authentication and role-based authorization.
- **CORS Support**: Configured for cross-origin requests from the frontend.

# Screenshots
![Index page](./images/screenshot.png) 

## Getting Started

### Prerequisites

- .NET SDK 8.0+
- Sqlite (or another preferred database)
- Minio
- [Aviato Bootstrap Frontend](https://github.com/themefisher/aviato-bootstrap) (optional for full-stack development)

### Installation

1. Clone the repository:

```bash
git clone https://github.com/amirkhaki/DrMarko.git
```

2. Navigate to the project directory:

```bash
cd DrMarko
```

3. Install the required dependencies (if using **Nix** with `flake.nix`):

```bash
nix develop
```

4. Set up the database connection string and other variables (admin account, minio instance) in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
}
```

5. Apply migrations and update the database:

```bash
dotnet ef database update
```

6. Run the application:

```bash
dotnet run
```

## Usage

1. Open your browser and navigate to:

```
https://localhost:5124/
```

2. The Aviato Bootstrap frontend should load with dynamic data coming from the backend.

## Contributing

If you'd like to contribute to this project, feel free to fork the repository and submit a pull request. Any contributions are welcome! 

## Contact

For questions or suggestions, feel free to contact:

- GitHub: [amirkhaki](https://github.com/amirkhaki)

# About the project name

The project name is based on Tim Marcoh from Fullmetal Alchemist
