# IT-IJ_AMS_DBTC

Group Members:
Aranzado, Martin Jonas A. - Backend Developer
Syllanto, Mary Jocelyn - Frontend Developer
Villareal, Ivan Vincent - Project Manager

**Attendance Management System**

1. ASP.NET Core Web API

This project is built using ASP.NET Core Web API for managing attendance data for students, teachers, and administrators.

2. Overview

The system allows schools or organizations to:

Track student attendance
Manage users (Admin, Teacher, Student)
Generate reports

3. Architecture & Layered Structure

Layered design separates responsibilities:

API Controllers – Handle HTTP requests/responses
Services – Core business logic
Repositories – Database interactions
Database – Stores users, attendance, and logs
[API Controllers] -> [Services] -> [Repositories] -> [Database]

4. Features

User login with roles (Admin, Teacher, Student)
Record and view attendance
Generate attendance reports
Notifications for absences
Manage user roles and permissions

5. Error Handling & Testing

Error Handling – Global exception handling with clear HTTP responses
Testing – Unit and integration tests for reliability

Common HTTP Status Codes:

200	OK	Attendance records retrieved
201	Created	New student added
400	Bad Request	Missing required fields
401	Unauthorized	User not logged in
403	Forbidden	Access denied
404	Not Found	Student/record not found
500	Internal Server Error	Server error

6. Example API Endpoints

Login: POST /api/auth/login
Get Attendance: GET /api/attendance/student/{id}
Add Attendance: POST /api/attendance
