# פרויקט Docker — Angular + .NET

## סקירה קצרה
פרויקט זה מכיל שתי דרכים להריץ את היישום:
1. **Dockerfile** — עבור בנייה וריצה ידנית
2. **docker-compose.yml** — עבור ניהול קל של container

---

## 📄 Dockerfile

### שלבי הבנייה (Multi-stage Build)

#### שלב 1: בנייה של ה־Angular Client
- בסיס: `node:20-alpine`
- התקנת dependencies: `npm ci --legacy-peer-deps`
- בנייה: `npm run build -- --configuration production --optimization=false`
- תוצאה: קבצי production בתיקייה `dist/client/browser`

#### שלב 2: בנייה של ה־.NET Server
- בסיס: `mcr.microsoft.com/dotnet/sdk:8.0`
- Restore dependencies: `dotnet restore`
- Publish: `dotnet publish -c Release`

#### שלב 3: Runtime
- בסיס: `mcr.microsoft.com/dotnet/aspnet:8.0`
- העתקת השרת ה־published
- העתקת קבצי Angular ל־`wwwroot`
- ריצה: `dotnet server.dll`

---

## 🐳 Docker Compose

קובץ `docker-compose.yml` מנהל את כל ההגדרות בקלות:

```yaml
version: '3.8'
services:
  app:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_URLS=http://+:80
    restart: unless-stopped
🚀 איך להשתמש
שימוש ב־Dockerfile ישירות:
docker build -t project-api-angular .
docker run -p 8080:80 project-api-angular
שימוש ב־Docker Compose:
docker-compose up --build
או ללא בנייה חדשה:
docker-compose up

🌐 גישה ליישום
לאחר ההרצה, גשו לכתובת:
http://localhost:8080

🛑 עצירה של הקונטיינר
עם Docker Compose:
docker-compose down

ללא Docker Compose:
docker stop <container-id>
