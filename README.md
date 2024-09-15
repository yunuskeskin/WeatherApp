# WeatherApp

WeatherApp is a web application that provides real-time weather information for various locations using the OpenWeatherMap API. This app allows users to register, log in, and retrieve current weather conditions based on their location. Authentication is handled via JWT tokens.

## Features

- User registration and login
- Token-based authentication (JWT)
- Fetch weather by username
- Display real-time weather data
- Caches data for better performance

## Technologies Used

- **C# (.NET Core)**: Backend logic and APIs.
- **ASP.NET Core Web API**: To handle API requests.
- **Entity Framework Core**: For data access and interaction with the database.
- **OpenWeatherMap API**: To fetch real-time weather data.
- **IMemoryCache**: For caching weather data.
- **nUnit/Moq**: For unit testing.
- **Swagger**: For API documentation and testing.

## Installation

### Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [OpenWeatherMap API Key](https://openweathermap.org/api)

### Steps

1. Clone the repository:
    ```bash
    git clone https://github.com/yunuskeskin/WeatherApp.git
    ```

2. Navigate to the project directory:
    ```bash
    cd WeatherApp
    ```

3. Install the required packages:
    ```bash
    dotnet restore
    ```

4. Set up the environment variable for your OpenWeatherMap API key:
    ```bash
    export OpenWeatherMapApiKey="your_api_key_here"
    ```

5. Run the application:
    ```bash
    dotnet run
    ```

6. Open your browser and go to:
    ```
    http://localhost:5000/swagger
    ```
    You can test the API via Swagger.

## Database
### Create Table
Connect a suitable database. And then create `Users` table.
Make sure that you committed your table to database.
```
CREATE TABLE Users (
	Id INTEGER PRIMARY KEY,
	FirstName TEXT NOT NULL,
	LastName TEXT NOT NULL,
	UserName TEXT NOT NULL,
	Email TEXT NOT NULL,
	PhoneNumber TEXT,
	Password TEXT NOT NULL, --Encrypted
	Address TEXT,
	BirthDate TEXT,
	LivingCountry TEXT,
	CitizenCountry TEXT,
	Salt TEXT --It's required to encrypte password
);
commit;
```

## Usage

### API Flow

#### 1. Registration (Create a User)
Send a `POST` request to `/api/registration` to register a new user:

**Request:**
```bash
POST /api/registration
Content-Type: application/json

{
  "FirstName": "isil",
  "LastName": "Keskin",
  "Password": "av7123Ae",
  "CountryCitizen":"TUR",
  "Email":"isil@gmail.com",
  "LivingCountry":"TUR",
  "Address":"Mardin",
  "PhoneNumber":"+053082915561",
  "BirthDate" : "15/06/1992"
}
```

**Response:** If successful, you will receive the registered user's name.
```bash
{
    "username": "ikeskin1"
}
```

#### Login (Authenticate the User)
Send a `POST` request to `/api/login` to log in with the registered user's credentials:

**Request:**
```bash
POST /api/registration
Content-Type: application/json
{
    "userName": "jdoe",
    "password": "password123"
}
```
**Response:** The response will include a JWT token. This token is required to authenticate further requests.
Example response:
```bash
{
    "token": "your-token-here"
}
```

#### 3. Get Weather Data (Authenticated Request)
Send a `GET` request to `/api/weather/{username}` to get the weather data for the registered user. Use the JWT token from the login response as a Bearer token in the Authorization header.

**Request:**
```bash
GET /api/weather/jdoe
Authorization: Bearer your_generated_jwt_token_here
```

**Response:** Weather data including city, temperature, and condition. 

Example response:
```bash
{
    "city": "Mardin",
    "temperature": "26,59°C",
    "condition": "clear sky",
    "lastUpdated": "2024-09-15T20:22:42.6481112+03:00"
}
```

## Running Tests

To test the registration, login, and weather API endpoints, follow these steps:

1. **Registration**:
   - First, send a `POST` request to `/api/registration` to create a user.

2. **Login**:
   - After successful registration, send a `POST` request to `/api/login` with the registered user's credentials.
   - You will receive a JWT token in the response.

3. **Fetch Weather Data**:
   - Use the JWT token from the login response as a Bearer token in the Authorization header to send a `GET` request to `/api/weather/{username}`.

4. To run unit tests:
    ```bash
       dotnet test
    ```
5. The tests use nUnit and Moq to verify the functionality of services and controllers.

## Project Structure
```bash
/WeatherApp
│
├── /Controllers             # API Controllers
├── /Core                    # Data models
├── /Services                # Business logic and API calls
├── /Tests                   # Unit tests
```


## API Endpoints

```

| Endpoint                    | Method | Description                                        |
|-----------------------------|--------|----------------------------------------------------|
| `/api/registration`         | POST   | Register a new user                                |
| `/api/login`                | POST   | Log in with username and password, returns JWT     |
| `/api/weather/{username}`   | GET    | Fetches the current weather data for the username  |
```

## Acknowledgements

- [OpenWeatherMap](https://openweathermap.org/) for the weather API.
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) for backend framework.
- [Swagger](https://swagger.io/) for API documentation.
