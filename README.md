# URL Shortener API

URL Shortener API is a simple and efficient microservice for shortening URLs, retrieving the original URLs, and tracking URL usage metrics. It leverages PostgreSQL, Redis, and OpenTelemetry to deliver a robust solution for URL shortening.

---

## Features

- **Shorten URLs**: Generate short codes for long URLs.
- **Retrieve Original URLs**: Use the short code to retrieve the original URL.
- **Track Visits**: Record visit metadata, including user-agent and referer headers.
- **Metrics Collection**: Collect and expose telemetry data using OpenTelemetry.
- **Scalability**: Hybrid cache (Redis + In-memory) for fast retrieval.

---

## Technologies Used

- **.NET 9.0**: Core backend language and framework.
- **PostgreSQL**: Primary database for persistent storage.
- **Redis**: Distributed caching layer.
- **Dapper**: Lightweight ORM for database interactions.
- **OpenTelemetry**: Observability for metrics and performance monitoring.
- **Swagger**: API documentation and testing.
