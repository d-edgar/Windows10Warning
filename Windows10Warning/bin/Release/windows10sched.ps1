# Define task name
$taskName = "Win10NotificationTask"

# Define the path to your application
$exePath = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\Windows10Warning.exe"

# Ensure the EXE exists before creating the task
if (!(Test-Path $exePath)) {
    Write-Host "Executable not found at: $exePath"
    exit 1
}

# Get the currently logged-in user
$loggedInUser = (Get-WMIObject -Class Win32_ComputerSystem | Select-Object -ExpandProperty UserName)

# Create a scheduled task action
$action = New-ScheduledTaskAction -Execute $exePath

# Create a trigger that repeats every 30 minutes indefinitely
$timeTrigger = New-ScheduledTaskTrigger -Once -At (Get-Date) `
    -RepetitionInterval (New-TimeSpan -Minutes 30) `
    -RepetitionDuration (New-TimeSpan -Days 3650)  # Runs indefinitely (10 years)

# Task settings
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable

# Register the task under the current user
Register-ScheduledTask -TaskName $taskName `
    -Action $action `
    -Trigger $timeTrigger `
    -Settings $settings `
    -User $loggedInUser `
    -RunLevel Limited

Write-Host "Scheduled task '$taskName' created successfully to run every 30 minutes."