# HairSalon — веб-приложение для управления салоном красоты

<div align="center">

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![Blazor](https://img.shields.io/badge/Blazor-Server-brightgreen)
![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-blue)
![SQL Server](https://img.shields.io/badge/MSSQL-2022-red)
![Docker](https://img.shields.io/badge/Docker-✓-2496ED)

**Курсовой проект по дисциплине «Кроссплатформенная среда исполнения программного обеспечения»**

</div>

---

## О проекте

**HairSalon** — это веб-приложение для автоматизации работы салона красоты. Разработано с использованием современного стека технологий .NET 8, ASP.NET Core, Blazor Server и MS SQL Server. Приложение позволяет клиентам записываться к мастерам, а администраторам и мастерам — управлять расписанием, услугами и пользователями.

### Основные возможности

#### Для клиентов:
- Регистрация и авторизация в системе
- Просмотр каталога услуг и мастеров
- Запись на прием к мастеру с выбором услуги
- Просмотр и отмена своих записей
- Личный кабинет с возможностью смены пароля

#### Для мастеров:
- Просмотр своего расписания
- Создание записей для клиентов
- Просмотр информации о клиентах

#### Для администраторов:
- Полное управление пользователями (CRUD)
- Управление мастерами и их привязка к пользователям
- Управление услугами (добавление, редактирование, удаление)
- Полный просмотр и управление всеми записями
- Изменение статуса пользователей

---

## Технологии

| Технология | Назначение |
|------------|------------|
| .NET 8 | Кроссплатформенная среда исполнения |
| ASP.NET Core | Веб-фреймворк |
| Blazor Server | Интерактивный веб-интерфейс на C# |
| Entity Framework Core 8 | ORM, Code First, миграции |
| MS SQL Server 2022 | Реляционная база данных |
| ASP.NET Core Identity | Аутентификация и авторизация |
| Docker / Docker Compose | Контейнеризация и оркестрация |
| Git | Система контроля версий |

---

## Структура проекта
```
HairSalon/
├── Controllers/Api/ # API контроллеры
│ ├── ConfigApiController.cs # Конфигурация приложения
│ ├── MastersApiController.cs # Управление мастерами
│ ├── ReceptionsApiController.cs # Управление записями
│ ├── ServicesApiController.cs # Управление услугами
│ └── UsersApiController.cs # Управление пользователями
├── Data/ # Доступ к данным
│ ├── DbSeeder.cs # Начальные данные (seed)
│ └── HairSalonContext.cs # Контекст БД + Fluent API
├── DTOs/ # Объекты передачи данных
│ ├── MasterDto.cs
│ ├── ReceptionDto.cs
│ ├── ServiceDto.cs
│ └── UserDto.cs
├── Models/ # Модели сущностей
│ ├── Master.cs # Мастер (связь с пользователем)
│ ├── Reception.cs # Запись к мастеру
│ ├── Service.cs # Услуга
│ └── User.cs # Пользователь
├── Services/ # Бизнес-логика
│ ├── MasterService.cs
│ ├── ReceptionService.cs
│ ├── ServiceService.cs
│ └── UserService.cs
├── Pages/ # Blazor страницы
│ ├── AdminAuth.razor # Вход для администратора
│ ├── AdminPanel.razor # Панель администратора
│ ├── Booking.razor # Запись на услугу
│ ├── Catalog.razor # Каталог услуг
│ ├── Index.razor # Главная страница
│ ├── MasterAuth.razor # Вход для мастера
│ ├── MasterPanel.razor # Панель мастера
│ ├── Masters.razor # Список мастеров
│ ├── MyReceptions.razor # Мои записи
│ ├── Profile.razor # Профиль пользователя
│ ├── Receptions.razor # Управление записями
│ ├── Services.razor # Управление услугами
│ └── Users.razor # Управление пользователями
├── Shared/ # Общие компоненты
│ └── MainLayout.razor # Основной макет
├── wwwroot/css/ # Стили
│ └── site.css # Основной CSS файл
├── appsettings.json # Конфигурация (БД, настройки)
├── appsettings.Development.json # Конфигурация для разработки
├── Program.cs # Точка входа, DI, миграции
├── Dockerfile # Docker-образ приложения
├── docker-compose.yml # Оркестрация контейнеров
├── .env # Переменные окружения
└── README.md # Документация
```


---

## Быстрый старт

### Предварительные требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (рекомендуется)
- [MS SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (при локальном запуске)
- [Git](https://git-scm.com/)

---

## Запуск через Docker (рекомендуемый способ)

Этот способ не требует установки .NET SDK и SQL Server на хост-машине.

### Шаг 1: Клонирование репозитория

git clone https://github.com/aresuku/Nazarick-CW-.git
cd HairSalon

### Шаг 2: Запуск контейнеров
docker-compose up -d

### Шаг 3: Открыть приложение
http://localhost:8023
```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                              БАЗА ДАННЫХ Nazaric                                            │
└─────────────────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────┐         ┌─────────────────────┐         ┌─────────────────────┐
│        USERS        │         │       MASTERS       │         │      SERVICES       │
├────┬────────────────┤         ├────┬────────────────┤         ├────┬────────────────┤
│ PK │ UserId         │◄────────│ FK │ UserId         │         │ PK │ ServiceId      │
├────┼────────────────┤  1:1    ├────┼────────────────┤         ├────┼────────────────┤
│    │ Login (UNIQUE) │         │ PK │ MasterId       │         │    │ Name (UNIQUE)  │
│    │ PasswordHash   │         │    │ FirstName      │         │    │ Price          │
│    │ Username       │         │    │ LastName       │         │    │ Description    │
│    │ Email (UNIQUE) │         │    │ Experience     │         └────┴─────┬──────────┘
│    │ Role           │         │    │ Gender         │                    │
│    │ IsActive       │         │    │ Description    │                    │
└────┴────┬───────────┘         │    │ Email (UNIQUE) │                    │
          │                     │    │ Role           │                    │
          │                     └────┴─────┬──────────┘                    │
          │ ClientId                       │                               │
          │ (FK)                           │ MasterId                      │ ServiceId
          │    ┌───────────────────────────┘ (FK)                          │ (FK)
          │    │ ┌─────────────────────────────────────────────────────────┘
          │    │ │   
          │    │ │                                                                   
          │    │ │       ┌────────────────────────────────────────────────────┐      
          │    │ │       │                   RECEPTIONS                       │      
          │    │ │       ├────┬───────────────────────────────────────────────┤      
          │    │ │       │ PK │ Id                                            │      
          │    │ │       ├────┼───────────────────────────────────────────────┤      
          │    └─┼──────→│ FK │ MasterId ─────────────────────────────────────┤      
          │      └──────→│ FK │ ServiceId ────────────────────────────────────┤      
          └─────────────→│ FK │ ClientId ─────────────────────────────────────┤      
                         │    │ Time ─────────────────────────────────────────┤      
                         └────┴───────────────────────────────────────────────┘      
                                                                                   
                                  УНИКАЛЬНЫЙ ИНДЕКС: (MasterId, Time)                   
```                    

