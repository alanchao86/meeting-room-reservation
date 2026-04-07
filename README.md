# Meeting Room Reservation

Small demo system for room booking:

`search rooms -> create reservation -> view my reservations -> cancel reservation`

## Tech Stack

- Frontend: Angular 18
- Backend: ASP.NET Core Web API (.NET 8)
- Database: SQLite (auto-created; seeded on startup)

## Project Structure

```text
backend/        ASP.NET API + SQLite data layer
frontend/       Angular SPA
docs/           Active product/UX/API/schema specs
documentation/  Additional reference docs
```

## Documentation (Quick Reference)

- DB schema: `docs/db-schema.md`
- API contract: `docs/api-endpoints.md`

## Core Scope & Rules

- Currently no login/auth (fixed `default-user`)
- Reservable window: `08:00-18:00`
- Slot size: `30 minutes`
- Reservation must be consecutive
- Max duration: `2 hours` (`4 slots`)

## Quick Start

### 1) Start Backend

```bash
cd backend/src/MeetingRoom.Api
dotnet run --urls http://localhost:5225
```

### 2) Start Frontend

```bash
cd frontend
npm install
npm start
```

Frontend runs on `http://localhost:4200` .
