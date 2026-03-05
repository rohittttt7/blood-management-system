# 🩸 Blood Bank Management System

A modern, feature-rich Blood Bank Management System built with ASP.NET Core MVC, Entity Framework Core, and SQL Server.

## ✨ Features

### 🎨 **Beautiful Modern UI/UX**
- ✅ Gradient backgrounds and smooth animations
- ✅ Responsive design for all devices
- ✅ Interactive hover effects
- ✅ Real-time form validation
- ✅ Auto-hiding notifications
- ✅ Loading animations
- ✅ Modern color schemes
- ✅ Font Awesome icons
- ✅ Bootstrap 5 components

### 👨‍💼 **Admin Features**
- Dashboard with real-time statistics
- Approve/Reject donor registrations
- Manage blood inventory (8 blood groups)
- Process blood requests (Emergency & Normal)
- Record blood donations
- Create and manage donation camps
- User management (Activate/Deactivate)
- Generate comprehensive reports
- View donation and distribution history

### 🩸 **Donor Features**
- Personal dashboard with donation stats
- Complete donation history
- Profile management
- View upcoming donation camps
- Check blood availability
- Donation eligibility tracking
- Next eligible donation date

### 🏥 **Patient Features**
- Request blood (Emergency & Normal)
- Track request status
- View blood availability
- Search donors by blood group
- View request history
- Cancel pending requests

## 🚀 How to Run the Project

### Prerequisites
- Visual Studio 2022 or later
- .NET 10.0 SDK
- SQL Server (LocalDB or SQL Server Express)

### Steps to Run

1. **Open the Project**
   - Open Visual Studio
   - File → Open → Project/Solution
   - Navigate to: `R:\blood-management-system\BloodBankManagementSystem\BloodBankManagementSystem.csproj`

2. **Restore NuGet Packages** (if needed)
   - Right-click on Solution → Restore NuGet Packages

3. **Run the Application**
   - Press **F5** or click the **Start** button (green play button)
   - The application will automatically:
     - Create the database
     - Apply migrations
     - Seed initial data
     - Create default admin account

4. **Access the Application**
   - The browser will open automatically
   - Default URL: `http://localhost:5000`

### 🔐 Default Login Credentials

**Admin Account:**
```
Email: admin@bloodbank.com
Password: Admin@123
```

## 📋 System Overview

### Database Schema
- **Users** - ASP.NET Identity tables
- **Donors** - Donor profiles and information
- **Patients** - Patient profiles and information
- **BloodGroups** - Blood inventory (A+, A-, B+, B-, O+, O-, AB+, AB-)
- **DonationRecords** - Blood donation history
- **BloodRequests** - Patient blood requests
- **BloodDistributions** - Blood distribution records
- **DonationCamps** - Blood donation camp information

### User Roles
1. **Admin** - Full system control
2. **Donor** - Can donate blood
3. **Patient** - Can request blood

## 🎨 UI/UX Features

### Custom CSS Features
- **Animations**: Fade in, slide, pulse, scale
- **Gradient Backgrounds**: Modern gradient color schemes
- **Hover Effects**: Interactive card and button animations
- **Responsive Design**: Mobile-first approach
- **Custom Scrollbar**: Themed scrollbar design
- **Loading States**: Beautiful loading animations
- **Form Validation**: Real-time validation feedback
- **Status Badges**: Color-coded status indicators

### JavaScript Enhancements
- Auto-hide alerts after 5 seconds
- Smooth scrolling
- Form loading states
- Real-time validation feedback
- Blood drop pulse animation
- Emergency request highlighting
- Number counter animations
- Tooltip initialization

## 📊 Key Workflows

### Blood Donation Workflow
1. Donor registers → Admin reviews → Admin approves
2. Donor donates blood → Admin records donation
3. Blood added to inventory
4. Next eligible date calculated (90 days)

### Blood Request Workflow
1. Patient requests blood
2. Admin reviews request
3. Admin checks inventory
4. Admin approves/rejects
5. Blood allocated and distributed
6. Inventory updated

### Emergency Request Priority
- Emergency requests displayed with warning badges
- Processed with higher priority
- Auto-highlighted in the system

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core MVC 10.0
- **ORM**: Entity Framework Core 10.0
- **Database**: SQL Server (LocalDB)
- **Authentication**: ASP.NET Core Identity
- **Frontend**: 
  - Bootstrap 5
  - jQuery
  - Font Awesome 6
  - Custom CSS3 animations
  - Modern JavaScript (ES6+)

## 📱 Responsive Design

The system is fully responsive and works on:
- 💻 Desktop (1920x1080+)
- 💻 Laptop (1366x768+)
- 📱 Tablet (768x1024)
- 📱 Mobile (320x568+)

## 🔧 Configuration

### Connection String
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BloodBankDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### To Change Database Server
Update the connection string in `appsettings.json` to point to your SQL Server instance.

## 📸 Screenshots

Visit the application to see:
- Modern landing page with blood availability
- Interactive dashboard with statistics
- Beautiful forms with validation
- Responsive tables with actions
- Animated cards and components

## 🎯 Future Enhancements

- SMS/Email notifications
- Blood donation certificate generation
- Mobile app integration
- QR code for donors
- Blood donation reminders
- Advanced analytics and charts
- Multi-language support
- Dark mode theme

## 👥 User Testing

### Test as Admin
1. Login with admin credentials
2. Approve donor registrations
3. Manage blood inventory
4. Process blood requests
5. Record donations

### Test as Donor
1. Register as a donor
2. Complete donor profile
3. Wait for admin approval
4. View dashboard
5. Check donation history

### Test as Patient
1. Register as a patient
2. Complete patient profile
3. Request blood
4. Check request status
5. View blood availability

## 📞 Support

For issues or questions:
- Check the application logs
- Review the database for data integrity
- Ensure all migrations are applied
- Verify NuGet packages are restored

## 📄 License

This project is created for educational and demonstration purposes.

## 🎉 Enjoy!

The Blood Bank Management System is ready to use with a modern, beautiful UI/UX design!

**Access the application at:** http://localhost:5000

---

**Built with ❤️ using ASP.NET Core MVC**
