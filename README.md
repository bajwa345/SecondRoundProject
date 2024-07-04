# SecondRoundProject
SecondRoundProject is a .NET 7 web application designed for managing clients with authentication, authorization, and some CRUD functionalities. It utilizes Dapper for database operations, Serilog for logging, FluentValidation for input validation, and JWT tokenization for secure authentication. The application assumes secure handling of sensitive data and efficient performance through optimized SQL queries and Redis caching for improved scalability.

Technical Details:

    Architecture: The application follows a layered architecture with clear separation of concerns, utilizing repositories for data access and services for business logic.
    Libraries Used:
        Dapper: Lightweight ORM for database operations.
        Serilog: Logging library for comprehensive application monitoring and error handling.
        FluentValidation: Provides robust input validation with custom rules for registration and client management.
        JWT Tokenization: Implements JWT tokens for secure user authentication and role-based access control.
        Redis: Utilized for caching last 3 search parameters to enhance application performance.
    Assumptions:
        Secure handling of user credentials and sensitive data.
        Efficient database queries and optimized performance.
        Proper authentication and authorization mechanisms are in place.
