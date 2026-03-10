# Devinity — Learning App

A cross-platform mobile learning platform built with **.NET MAUI** (frontend) and **ASP.NET Core** (backend API), deployed on **Railway**. Users can enroll in programming courses, track progress, earn certificates, and compete on a leaderboard.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Mobile App | .NET MAUI (C#, XAML) |
| Backend API | ASP.NET Core 9 |
| Database | MySQL (Railway) |
| ORM | Entity Framework Core |
| File Storage | Railway Volume (`/app/uploads`) |
| Video Hosting | Firebase Storage |
| Auth | JWT + Refresh Tokens |
| PDF Generation | SkiaSharp |
| Animations | SkiaSharp Lottie |
| Deployment | Railway |

---

## Features

### Authentication
- Register and login with email and password
- JWT access tokens + refresh token rotation
- Secure token storage via MAUI `SecureStorage`
- Password visibility toggle on login and register pages

### Home / Main Page
- Daily motivational quote popup (2 second delay after login)
- Course category grid (PHP, Python, JavaScript, Java, C#, C++, C, MySQL)
- Tap a category to enter the course detail page

### Course Detail
- Firebase-hosted video player via `MediaElement`
- Video progress tracking (per video, per user)
- Assessment completion tracking

### My Learning
- In Progress / Completed tab switcher
- Course cards with PNG language icons, progress bar, hours estimate
- Skeleton loading placeholders
- Tap completed course → Certificate page
- Tap in-progress course → Course detail page

### Certificates
- Auto-generated PDF certificate using SkiaSharp
- A4 landscape layout with course name, user name, and completion date
- Download / share from the app

### Leaderboard
- Top 10 users ranked by score
- Score formula: `courses × 100 + hours × 10 + assessments × 5`
- Podium UI for top 3 (gold, silver, bronze)
- Avatar images loaded from Railway volume URLs
- Current user highlighted

### Profile
- Avatar upload (JPEG/PNG/WebP, max 5MB)
- Stored on Railway volume, served as static files
- Display name, email, student badge
- Stats: courses completed, hours watched, certificates earned
- Settings bottom sheet (slide-up animation)
- Edit profile: change name and/or password

---

## API Endpoints

### Auth
| Method | Route | Description |
|---|---|---|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login, returns JWT + refresh token |
| POST | `/api/auth/refresh` | Refresh access token |
| PUT | `/api/auth/profile` | Update name / password |
| POST | `/api/auth/avatar` | Upload avatar image |

### Learning
| Method | Route | Description |
|---|---|---|
| GET | `/api/learning/{userId}/overview` | Course progress overview |
| POST | `/api/learning/video-progress` | Mark video watched |
| POST | `/api/learning/assessment` | Record assessment completion |

### Leaderboard
| Method | Route | Description |
|---|---|---|
| GET | `/api/leaderboard` | Top 10 users by score |

---

## Setup & Deployment

### Prerequisites
- .NET 9 SDK
- Visual Studio 2026 with MAUI workload
- MySQL database (Railway or local)
- Railway account for API hosting

### Environment Variables (Railway)
```
ConnectionStrings__DefaultConnection=Server=...;Database=...;User=...;Password=...
JwtSettings__SecretKey=your-secret-key-min-32-chars
JwtSettings__Issuer=LearningApp
JwtSettings__Audience=LearningAppUsers
PORT=8080
```

### Railway Volume
Mount a volume at `/app/uploads`. The API serves avatars as static files at:
```
https://devinity-production.up.railway.app/uploads/avatars/{userId}.jpg
```

### MAUI NuGet Packages
```
CommunityToolkit.Maui
CommunityToolkit.Maui.MediaElement
SkiaSharp.Extended.UI.Maui
Xamarin.AndroidX.LocalBroadcastManager (Android)
```

### API NuGet Packages
```
Microsoft.EntityFrameworkCore
Pomelo.EntityFrameworkCore.MySql
Microsoft.AspNetCore.Authentication.JwtBearer
BCrypt.Net-Next
```

### Run Locally
```bash
# API
cd LearningApp.Api
dotnet run

# MAUI (update AppConfig.cs BaseUrl to your local IP)
# Run from Visual Studio targeting Android emulator or device
```

---

## Image Assets Required

Place these in `LearningApp/Resources/Images/` with Build Action `MauiImage`:

```
php.png, python.png, javascript.png, java.png
csharp.png, cplusplus.png, c.png, mysql.png
eye.png, eye_off.png
setting.png, edit.png, notification.png
privacy.png, ic_help.png, leaderboard.png, meetings.png
```

---

## Scoring System

| Action | Points |
|---|---|
| Course completed | +100 |
| Hour of video watched | +10 |
| Assessment completed | +5 |

---

## License

Private project — all rights reserved.

