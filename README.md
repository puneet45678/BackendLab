# BackendLab — Production-Grade .NET Microservices

A hands-on learning project covering real backend engineering concepts in one integrated system — not isolated toy implementations.

Built with **C# ASP.NET Core 8**, following production patterns a real team would use.

---

## What This Project Covers

| Concept | Implementation |
|---|---|
| Clean Architecture | 4-layer structure per service (Domain / Application / Infrastructure / API) |
| CQRS + MediatR | Commands, Queries, Handlers in the Application layer |
| Multi-tenancy | Row-level isolation — `TenantId` on all entities, EF Core global query filters |
| Authentication | JWT + Refresh token rotation, OAuth2 |
| Authorization | RBAC — policy-based, role claims in JWT |
| Global error handling | `IExceptionHandler` + `ProblemDetails` (RFC 9457) |
| Rate limiting | Nginx (`limit_req_zone`) + ASP.NET Core `System.Threading.RateLimiting` |
| gRPC | Sync inter-service calls with `.proto` contracts |
| Async messaging | MassTransit + RabbitMQ, Outbox pattern for reliable delivery |
| Distributed Saga | MassTransit `StateMachine` — orchestration with compensation |
| OpenTelemetry | Distributed traces across HTTP + gRPC, metrics to Prometheus |
| Structured logging | Serilog with correlation ID enricher, Seq for log aggregation |
| Real-time | SignalR with Redis backplane for order status push |
| Resilience | `Microsoft.Extensions.Http.Resilience` — retry, circuit breaker, timeout |
| Health checks | Liveness + readiness probes per service |
| API versioning | URL segment (`/v1/`, `/v2/`) |
| Docker | Multi-stage builds, non-root user, health checks |
| Kubernetes | Deployments, HPA, network policies, resource limits |
| Service mesh | Istio — mTLS, traffic management, circuit breaking at infra level |
| CI/CD | GitHub Actions — build, test, Docker build/push, deploy |

---

## Architecture

### Services

```
api-gateway          → Nginx — reverse proxy, rate limiting, SSL termination
auth-service         → JWT issuing, refresh tokens, RBAC
user-service         → User profiles, gRPC server
order-service        → Orders, hosts OrderSaga, outbox pattern
inventory-service    → Stock reservation, saga participant
payment-service      → Payment processing, saga participant
notification-service → Consumes events, SignalR hub
```

### Per-Service Structure (Clean Architecture)

```
ServiceName/
  src/
    ServiceName.Domain/          # Entities, Value Objects, Domain Events — zero dependencies
    ServiceName.Application/     # CQRS handlers, DTOs, FluentValidation, Result<T>
    ServiceName.Infrastructure/  # EF Core, Repositories, MassTransit, Redis
    ServiceName.API/             # Minimal APIs, Middleware, Program.cs
  Dockerfile
```

### The Order Flow (Saga)

```
User places order
  └─ OrderSaga kicks in (MassTransit StateMachine)
       ├─ 1. Validate user → user-service (gRPC)
       ├─ 2. Reserve stock → inventory-service (async)
       ├─ 3. Process payment → payment-service (async)
       ├─ 4. Confirm order
       └─ Any step fails → compensation runs (release stock, refund)

Order status pushed to client in real-time via SignalR
```

### Multi-tenancy

Row-level isolation — every entity has a `TenantId`. EF Core global query filters automatically scope all queries. Tenant resolved from `tenant_id` JWT claim at the API layer.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# ASP.NET Core 8, Minimal APIs |
| ORM | EF Core + Postgres |
| Cache | Redis |
| Mapping | Mapster |
| Messaging | MassTransit + RabbitMQ |
| Resilience | `Microsoft.Extensions.Http.Resilience` |
| Logging | Serilog → Seq |
| Tracing | OpenTelemetry → Jaeger |
| Metrics | OpenTelemetry → Prometheus → Grafana |
| Real-time | SignalR (Redis backplane) |
| Gateway | Nginx |
| Containers | Docker + Docker Compose |
| Orchestration | Kubernetes + Istio |
| CI/CD | GitHub Actions |

---

## Running Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Start infrastructure

```bash
docker compose up -d
```

This starts Postgres, Redis, RabbitMQ, Seq, Jaeger, Prometheus, and Grafana.

| Service | URL |
|---|---|
| RabbitMQ UI | http://localhost:15672 — `guest` / `guest` |
| Seq (logs) | http://localhost:8081 — `admin` / `admin123` |
| Jaeger (traces) | http://localhost:16686 |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 — `admin` / `admin` |

### Run a service

```bash
dotnet run --project src/Services/UserService/UserService.API
```

### Run migrations

```bash
dotnet ef database update \
  --project src/Services/UserService/UserService.Infrastructure \
  --startup-project src/Services/UserService/UserService.API
```

---

## Key Concepts Explained

### Why Clean Architecture?
Dependencies point inward only — Domain has zero external dependencies. Swapping Postgres for another database only touches Infrastructure. Business logic never leaks into the API layer.

### Why Outbox Pattern?
Publishing an event and saving to the database are two separate operations. If the app crashes between them, you lose the event. The outbox writes both atomically in one transaction — a background worker then publishes from the outbox to RabbitMQ.

### Why Saga Orchestration over Choreography?
With choreography, the full order flow is implicit — spread across multiple services reacting to events. Hard to debug, hard to add compensation. Orchestration puts the full state machine in one place: you can read the flow top to bottom.

### Why two rate limiters (Nginx + app)?
Nginx rate limiting stops requests before they consume any application resources — protection at the network edge. App-level rate limiting gives you fine-grained control per user, per tenant, per endpoint.

### Why liveness AND readiness health checks?
Liveness: is the process alive? If no → Kubernetes restarts the pod.
Readiness: can it serve traffic? (DB connected, dependencies up) If no → Kubernetes removes it from the load balancer but doesn't restart it.

---

## Project Status

See the full build progress and phase checklist in the [Obsidian tracking note](docs/project-tracking.md) or check the commit history.

| Phase | Description | Status |
|---|---|---|
| 1 | Solution scaffold + Docker infra + BaseEntity + Tenant | 🔄 In Progress |
| 2 | AuthService + UserService — JWT, RBAC, CRUD | ⬜ |
| 3 | Nginx + Rate Limiting | ⬜ |
| 4 | gRPC + OrderService | ⬜ |
| 5 | OpenTelemetry | ⬜ |
| 6 | Async Messaging + Outbox | ⬜ |
| 7 | Saga (Distributed Transactions) | ⬜ |
| 8 | SignalR (Real-time) | ⬜ |
| 9 | Kubernetes | ⬜ |
| 10 | Service Mesh (Istio) | ⬜ |
| 11 | CI/CD | ⬜ |

---

## Purpose

This is a **learning lab**, not a portfolio project. Every decision is made the way a real team would make it. The goal is to understand what's happening behind the scenes at every layer — not just make something run.
