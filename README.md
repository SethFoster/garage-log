# garage-log
A personal car modification & maintenance tracker.

Features
- Dashboard with car selector, totals, progress bar, and mod list
- Filters by status and category, sort by cost/date
- Attach photos/receipts (paths stored) and notes/build log
- Lightweight .NET Minimal API backend with SQLite

Quick start (development)

1. Backend

```powershell
cd backend
dotnet build
dotnet run
# Backend listens on http://localhost:5084 by default (see launchSettings)
```

2. Frontend

```bash
cd frontend
npm install
npm run dev
# Open http://localhost:3000
```

Notes
- The frontend reads API base URL from `NEXT_PUBLIC_API_URL` (set `.env.local`).
- I added seed data and a simple CORS policy for local development.
- The app supports both "mod" and "maintenance" items; maintenance entries are seeded as Category = `Maintenance`.

Next steps
- Add photo/receipt upload endpoints and storage
- Add charts (cost by category) and PDF export

Contributing
- Fork, make changes, and open a PR. I can help wire up CI and tests if desired.

