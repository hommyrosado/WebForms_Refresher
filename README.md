# WebForms_Refresher
WebForms_Refresher
Great screenshot 👌 — you’ve got a **standard ASP.NET Web Forms project structure** in Visual Studio. Here’s a description of each item in your project tree so you can explain it cleanly in your interview:



Added SQLite ADO.NET
---


---

## **Project Root (`WebformsDemonstration`)**

This is your main **ASP.NET Web Forms application project**.

### **Connected Services**

* Lets you connect the app to external services (e.g., Azure Storage, REST APIs, WCF, etc.).

### **Properties**

* Contains project-level settings like **assembly info**, resources, and build settings.

### **References**

* Lists the external assemblies (DLLs) the project depends on (e.g., System.Web, Entity Framework, or NuGet packages).

---

## **Folders**

### **App\_Data**

* Special folder for **data storage** (e.g., `.mdf` databases, `.sqlite` files, XML).
* Content here is **not served** directly by IIS, making it safe for DB files.

### **App\_Start**

* Stores config classes (e.g., RouteConfig, BundleConfig, AuthConfig).
* These are executed early in the app lifecycle when the application starts.

### **Content**

* Holds **CSS files, images, static content**.

### **Scripts**

* Contains **JavaScript files** (client-side functionality, libraries like jQuery).

---

## **Pages & UI**

### **About.aspx, Contact.aspx, Default.aspx**

* **ASP.NET Web Forms pages** with `.aspx` markup (UI layer).
* Each has an optional code-behind file (`.aspx.cs`) for server-side logic.
* **Default.aspx** is usually the homepage.

### **Site.Master**

* A **master page** (template) that defines a consistent layout (header, nav, footer).
* Pages like `Default.aspx` and `Contact.aspx` use it via `<%@ MasterPageFile="..." %>`.

### **Site.Mobile.Master**

* A master page designed for **mobile devices**, enabling adaptive UI.

### **ViewSwitcher.ascx**

* A **user control** (`.ascx`) that can be embedded in pages.
* This one likely handles **switching between mobile/desktop views**.

---

## **Configuration & Metadata**

### **Bundle.config**

* Configures **bundling & minification** for scripts and styles (improves performance).

### **Dockerfile**

* Defines how to **containerize the Web Forms app** (e.g., build an IIS + .NET Framework Docker image).

### **favicon.ico**

* The site’s **browser tab icon**.

### **Global.asax**

* The **application file**: handles high-level events (Application\_Start, Session\_Start, Application\_Error).
* Runs once for the entire application, not per page.

### **packages.config**

* Tracks NuGet package dependencies for this project.

### **Web.config**

* The **main configuration file**:

  * Connection strings
  * Authentication/authorization settings
  * Session state configuration
  * App settings

---

## **Second Project (`WebformsDemonstration.Tests`)**

This is a **test project** to validate the Web Forms app.

### **Properties**

* Test project settings.

### **References**

* Assemblies/packages for testing (e.g., MSTest, NUnit, xUnit).

### **packages.config**

* NuGet packages specific to the test project.

### **UnitTest1.cs**

* A starter unit test class (default scaffold from Visual Studio).

---

✅ Together, this structure shows: **data-driven pages (App\_Data + .aspx)**, **UI templating (Master Pages + User Controls)**, **config (Web.config + Bundle.config)**, and **containerization (Dockerfile)**. It also demonstrates that you know how testing is separated (second project).

---
