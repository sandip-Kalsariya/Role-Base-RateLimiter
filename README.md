# Role-Based Rate Limiter

## Overview
This project implements a **Role-Based Rate Limiter** in a .NET 8 Web API application. It allows different roles to have different rate limits, ensuring fair resource usage and preventing abuse.



## Features
- **Role-Based Access Control**: Assign different rate limits to different user roles.
- **Configurable Rate Limits**: Customize limits per role.
- **Middleware Implementation**: Uses .NET 8 middleware for rate limiting.
- **JWT Authentication**: Ensures secure access control.
- **Logging & Monitoring**: Logs requests exceeding rate limits.

## Technologies Used
- **.NET 8 Web API**
- **ASP.NET Core Identity**
- **JWT Authentication**
- **EF Core & SQL Server**

## Installation & Setup
### Prerequisites
- .NET 8 SDK
- SQL Server

### Steps to Run the Project
1. **Clone the repository**
   ```sh
   git clone https://github.com/sandip-Kalsariya/Role-Base-RateLimiter.git
   cd Role-Base-RateLimiter
   ```
2. **Update the database connection string**
   - Modify `appsettings.json` with your SQL Server connection string.
3. **Apply Migrations**
   ```sh
   dotnet ef database update
   ```
4. **Run the application**
   ```sh
   dotnet run
   ```

## Configuration
The rate limits for each role are configurable in `appsettings.json`:
```json
 "RateLimitSettings": {
   "AdminRequestsPerHour": 1000,
   "UserRequestsPerHour": 100,
   "GuestRequestsPerHour": 10,
   "GlobalRequestsPerHour": 5000,
   "DynamicIncreaseAmount": 10
 }
```

## Login Users
- ###  Admin User
    **Username =** `"admin"`
    **Password =** `"admin123"`  
- ###  Regular User
    **Username =** `"user"`
    **Password =** `"user123"`  
- ###  Guest User
    **Username =** `"guest"`
    **Password =** `"guest123"`  

## API Endpoints
### Authentication
- **Login**: `/api/auth/login` (POST)
- **Register**: `/api/auth/register` (POST)

### Role-Based Rate-Limited Endpoints
- **General API Request**: `/api/Test/status` (GET)
  - Accessible with rate limits based on the user's role.

## License
This project is licensed under the MIT License.

Feel free to contribute or raise an issue if you find any bugs or need improvements! 🚀

