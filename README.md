# Customer Management System – ASP.NET Core MVC (Database First)

A simple customer management system built with **ASP.NET Core MVC** using **Entity Framework Core – Database First**.

---

## 📋 Features
- **Role-based login and permissions**:
  - **Admin**: Add / Edit / Delete / View / Print customers
  - **Manager**: View / Print customers only
  - **Employee**: Add / Edit / View customers only
  - The UI changes dynamically based on the user’s Role.
- Full CRUD operations for customers.
- Dynamic dropdowns: Governorate → District → Village, plus Gender.
- Automatically calculates Birth Date and Age from National ID.
- Full field validation.


---

## 🗂️ Project Structure
- **Controllers**: AccountController, CustomersController  
- **Views**: Customers (Index, Create, Edit, Delete, Print) + Account (Login)  
- **Models**: Users, Roles, Customers, Genders, Governorates, Districts, Villages  
- **Database**: SQL Server (Database First)

---

## ⚙️ Requirements
- **Visual Studio 2022** or later  
- **.NET 6.0** or later  
- **SQL Server**  

---

## 📝 Setup Instructions

### 1️⃣ Restore the Database
In the `Database` folder of the project you’ll find:
- `DatabaseScript.sql` (script to create tables and insert data)  
or  
- `DatabaseBackup.bak` (backup file)  

Do one of the following:
- Run the script in SQL Server Management Studio to create the database.  
- Or restore the `.bak` file.

> Make sure the database name in SQL Server matches the name in `appsettings.json`.

---

### 2️⃣ Configure the Connection String
Open `appsettings.json` and update the connection string to match your environment:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=CustomerDB;Trusted_Connection=True;"
}
````

---

### 3️⃣ Run the Project

Open the project in Visual Studio:

* From Package Manager Console or Terminal:

```bash
dotnet restore
dotnet build
dotnet run
```

Or just press **F5** to run.

---

## 🔑 Default Login Credentials

| Username | Password    | Role     |
| -------- | --------    | -------- |
| admin    | admin123    | Admin    |
| manager1 | manager123  | Manager  |
| employee | 1234        | Employee |

---
## ✅ Validation Rules

### 🔹 Customer Form Validation  
- **Full Name**  
  - Required.  
  - Must contain letters only (Arabic or English).  
  - Numbers and special characters are not allowed.  

- **National ID**  
  - Required.  
  - Must be **exactly 14 digits**  — no letters or special characters.
  - Must be **unique** (no duplicates).  
  - Cannot be edited after the record is created.  
  - Birth Date and Age are calculated automatically from it.
  - Century digit — first digit must be 2 (1900s) or 3 (2000s).
  - Date extraction — reads year, month, and day from the ID. If the date part is invalid, shows: “Invalid National ID date (month/day out of range).”

- **Governorate / District / Village**    
  - District dropdown filters automatically based on selected Governorate.  
  - Village dropdown filters automatically based on selected District.  

- **Gender**  
  - Must be selected from the dropdown (Male / Female).  

- **Salary**  
  - Must be a number between **5000** and **20000**.  

- **Birth Date**  
  - Read-only.  
  - Automatically calculated from National ID.  

- **Age**  
  - Read-only.  
  - Automatically calculated from Birth Date.  

---

### 🔹 Login Validation  
- **Username** and **Password** must match a record in the database.  
- User access is restricted by Role (Admin / Manager / Employee).  
- Unauthorized pages redirect to the Login page.  

---

### 🔹 Roles Restrictions  
- **Admin**: Can Add / Edit / Delete / View / Print customers.  
- **Manager**: Can View / Print customers only (cannot add/edit/delete).  
- **Employee**: Can Add / Edit / View customers (cannot delete/print).  


## 🗒️ Notes

* The UI changes dynamically based on the user’s Role.



