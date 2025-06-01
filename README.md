# 🎬 Cinema Management Web App

Aplikacja webowa do zarządzania kinem — napisana w ASP.NET Core Razor Pages z wykorzystaniem Bootstrap, Identity, PostgreSQL i Docker.

## ⚙️ Uruchamianie

1. **Zbuduj i uruchom kontener z bazą danych**:

   ```bash
   docker-compose up -d
   ```

2. **Uruchom aplikację**:

   ```bash
   dotnet run
   ```

✅ Migracje i seedowanie danych wykonują się automatycznie przy starcie aplikacji.

---

## 📁 Struktura Stron

```
Pages/
├── Account
│   ├── Index.cshtml           # Dane użytkownika
│   ├── Tickets.cshtml         # Zakupione biletów
│   └── AccessDenied.cshtml    # Dostęp zabroniony
├── Admin
│   ├── Movies.cshtml          # Zarządzanie filmami
│   ├── Screenings.cshtml      # Zarządzanie seansami
│   └── Users.cshtml           # Zarządzanie kontami
├── Login.cshtml               # Logowanie
├── Register.cshtml            # Rejestracja
├── Pricing.cshtml             # Cennik
├── Screening.cshtml           # Rezerwacja seansu
├── Upcoming.cshtml            # Nadchodzące filmy
├── Error404.cshtml            # Strona błędu 404
└── Shared/_Layout.cshtml      # Layout aplikacji
```

---

## 🔑 Uwierzytelnianie i Role

* Rejestracja/logowanie użytkownika
* Panel konta (`/Account/Index`) z edycją danych
* Role:

  * `User` – klient
  * `Admin` – dostęp do panelu administracyjnego

---

## 🧩 Technologie

* **ASP.NET Core Razor Pages**
* **Bootstrap 5**
* **PostgreSQL** (z Dockerem)
* **Entity Framework Core** + migracje + seedowanie
* **ASP.NET Identity**

---

Jasne! Poniżej znajduje się zaktualizowana sekcja ze zrzutami ekranu — z podziałem na kategorie, gdzie **każdy screen zajmuje pełną szerokość**, a **opis znajduje się pod obrazem**. To rozwiązanie świetnie sprawdzi się na GitHubie lub w plikach Markdown renderowanych w przeglądarce.

---

## 🖼️ Zrzuty Ekranu

### 🔐 Uwierzytelnianie

![LoginPage](Screenshots/LoginPage.png)
**Ekran logowania użytkownika**

---

![LoginPageAuthRequired](Screenshots/LoginPageAuthRequired.png)
**Komunikat o wymaganym zalogowaniu do uzyskania dostępu**

---

![LoginInvalidAttempt](Screenshots/LoginInvalidAttempt.png)
**Nieudana próba logowania z błędnymi danymi**

---

![RegisterPage](Screenshots/RegisterPage.png)
**Formularz rejestracji nowego użytkownika**

---

![RegisterRequiredFields](Screenshots/RegisterRequiredFields.png)
**Walidacja pól obowiązkowych podczas rejestracji**

---

### 👤 Konto Użytkownika

![AccountPage](Screenshots/AccountPage.png)
**Panel użytkownika z możliwością edycji danych**

---

![AccountPageRequiredFields](Screenshots/AccountPageRequiredFields.png)
**Walidacja pól obowiązkowych w panelu konta**

---

![TicketsPage](Screenshots/TicketsPage.png)
**Lista zakupionych biletów przypisanych do konta**

---

### 🎟️ Rezerwacja i Seanse

![ScreeningBookingPage](Screenshots/ScreeningBookingPage.png)
**Rezerwacja miejsc na wybrany seans filmowy**

---

![ScreeningBookingMaxSeatsSelected](Screenshots/ScreeningBookingMaxSeatsSelected.png)
**Komunikat o przekroczeniu limitu miejsc do rezerwacji**

---

![LandingPageScreeningUnavailable](Screenshots/LandingPageScreeningUnavailable.png)
**Brak dostępnych seansów dla wybranego filmu**

---

### 🏷️ Strony Ogólne

![LandingPage](Screenshots/LandingPage.png)
**Strona główna aplikacji z listą filmów i dostępnych seansów**

---

![PricingPage](Screenshots/PricingPage.png)
**Cennik biletów**

---

![UpcomingMoviesPage](Screenshots/UpcomingMoviesPage.png)
**Lista nadchodzących premier filmowych**

---

![Error404](Screenshots/Error404.png)
**Strona błędu 404 – brak zasobu**

---

![AccessDeniedPage](Screenshots/AccessDeniedPage.png)
**Ekran informujący o braku uprawnień (403)**

---

### 🛠️ Panel Administratora

![AdminAccountMenu](Screenshots/AdminAccountMenu.png)
**Menu administratora dostępne po zalogowaniu**

---

![AdminManageMovies](Screenshots/AdminManageMovies.png)
**Lista filmów z opcją dodawania, edytowania i usuwania**

---

![AdminMoviesAdd](Screenshots/AdminMoviesAdd.png)
**Formularz dodawania nowego filmu**

---

![AdminMoviesEdit](Screenshots/AdminMoviesEdit.png)
**Edycja danych filmu**

---

![AdminMoviesDeleteConfirm](Screenshots/AdminMoviesDeleteConfirm.png)
**Potwierdzenie usunięcia filmu**

---

![AdminManageScreenings](Screenshots/AdminManageScreenings.png)
**Zarządzanie seansami filmowymi**

---

![AdminScreeningsEdit](Screenshots/AdminScreeningsEdit.png)
**Edycja istniejącego seansu**

---

![AdminScreeningAddRoomOccupiedError](Screenshots/AdminScreeningAddRoomOccupiedError.png)
**Obsługa konfliktu – próba dodania seansu w zajętej sali**

---

![AdminScreeningsFilter](Screenshots/AdminScreeningsFilter.png)
**Filtrowanie seansów według filmu, sali lub daty**

---

![AdminManageUsers](Screenshots/AdminManageUsers.png)
**Zarządzanie kontami użytkowników – przegląd i filtrowanie**

---

![AdminUserEdit](Screenshots/AdminUserEdit.png)
**Edycja danych użytkownika przez administratora**

---

![AdminUserDeleteConfirm](Screenshots/AdminUserDeleteConfirm.png)
**Potwierdzenie usunięcia użytkownika**

---

![AdminUserFiltering](Screenshots/AdminUserFiltering.png)
**Filtrowanie kont użytkowników według e-maila, imienia, nazwiska i numeru telefonu**

---

## 📦 Docker & Baza Danych

Aplikacja korzysta z PostgreSQL uruchamianego w kontenerze.

### `docker-compose.yml`

Plik konfiguracyjny tworzy usługę `db`:

```yaml
version: '3.8'
services:
  db:
    image: postgres
    container_name: cinema_db
    environment:
      POSTGRES_USER: ...
      POSTGRES_PASSWORD: ...
      POSTGRES_DB: ...
    ports:
      - "5432:5432"
    volumes:
      - db-data:/var/lib/postgresql/data
volumes:
  db-data:
```

Po uruchomieniu aplikacja wykona migracje oraz zasieje bazę danymi początkowymi.

---

## 📌 Uwagi Końcowe

* W aplikacji obsługiwane są błędy 404 oraz brak dostępu
* W pełni responsywny interfejs dzięki Bootstrap 5
* Wszystkie formularze walidowane zarówno po stronie klienta, jak i serwera

