# Windows10Warning

## Overview

This program is a Windows Forms application that notifies users about the upcoming end-of-life for Windows 10. The notification prompts users to take action, either by upgrading their system or scheduling a consultation with IT.

Key Features

	•	🚀 Persistent Notification: Pops up a warning message reminding users to upgrade Windows 10.
	•	🔗 Actionable Button: Opens a customizable upgrade scheduling URL in the user’s default browser.
	•	❌ Dismiss for the Day: Users can click “Dismiss”, and the notification will not reappear until the next day.
	•	📁 Configuration Support: Uses a config.json file to define the title, URL, and logo dynamically.
	•	🔍 Auto-Closing Logic: If dismissed, the app automatically closes on launch without showing the notification.
	•	🖼 Custom Branding: Displays an organization-specific logo from a local file.

How It Works

	1. On Startup:
	• The app checks if a dismissal file exists (Win10NotificationDismissed.txt in AppData).
	• If the file contains today’s date, the app closes immediately.
	• If no dismissal file exists or the date is outdated, the notification appears.
	2. User Options:
	• Click “Schedule an Upgrade” → Opens the configured upgrade scheduling URL.
	• Click “Dismiss” → Saves today’s date to prevent the notification from showing again until the next day.
	3. Reopens Every 30 Minutes (via Scheduled Task)
	• If the notification was dismissed, it won’t appear again until the next day.
	• If not dismissed, it continues reminding users every 30 minutes (or as configured).

Customization

	• config.json allows easy customization:
{
    "FormTitle": "Windows 10 Upgrade Required",
    "RedirectUrl": "https://your-it-support-portal.com",
    "LogoPath": "logo.png"
}
	• Deployable via Scheduled Task to run on login and every 30 minutes.

Deployment
	•	The .exe should be placed in the All Users Startup Folder (C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\).
	•	A PowerShell script can create a Scheduled Task to launch the app at login and every 30 minutes.

🔹 Why Use This Program?

✅ Ensures users are reminded about the Windows 10 EOL deadline.
✅ Prevents notification fatigue by allowing daily dismissals.
✅ Automates IT communication with minimal intervention.
✅ Simple config-based customization for different environments.

Let me know if you need anything else for your README! 🚀
