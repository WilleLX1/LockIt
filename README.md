# LockIt

LockIt is a Windows application designed to monitor and control the execution of specific processes. It allows users to lock selected applications, preventing them from running without proper authentication. The application is especially useful for ensuring that certain processes remain restricted or require a password to unlock.

## Features

- **Process Monitoring:** Continuously monitors the system for specified processes.
- **Application Locking:** Prevents locked applications from running.
- **Password Protection:** Requires a password to unlock and relaunch locked applications.
- **Real-Time Notifications:** Logs application activity and provides alerts for unauthorized attempts.
- **User-Friendly Interface:** Simple UI for managing locked applications and viewing logs.

## How It Works

1. **Add Applications to Lock:** Specify the application process name (e.g., `notepad`) and set a password.
2. **Monitor Processes:** The application uses Windows Management Instrumentation (WMI) to monitor process start events.
3. **Close Unauthorized Apps:** If a locked application attempts to start, it is immediately closed.
4. **Password Prompt:** Users can unlock the application by entering the correct password.

## Technologies Used

- **C#**: Core programming language.
- **Windows Forms**: For creating the user interface.
- **System.Management**: For monitoring process start events.

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/WilleLX1/LockIt.git
   ```
2. Open the project in Visual Studio.
3. Build the solution.
4. Run the application.

## Usage

1. Launch the application.
2. Click on `Add Application` to specify a process to lock (e.g., `notepad`).
3. Monitor the log for real-time updates.
4. If the locked application starts, it will be terminated, and a password prompt will appear.
5. Enter the correct password to unlock and relaunch the application.

## Example

### Adding a Locked Application
- Enter the process name (e.g., `notepad`).
- Set a password (e.g., `1234`).
- Click `Add Application`.

### Attempting to Open a Locked Application
- The application will detect the process and immediately terminate it.
- A password prompt will appear. Enter the correct password to proceed.

## Known Issues

- The application requires administrative privileges to manage some system processes.
- Password prompts are handled in the main thread and may block other interactions temporarily.

## Future Enhancements

- **Multi-User Support:** Allow different users to have unique locked app configurations.
- **Advanced Logging:** Include detailed timestamped logs and export options.
- **Cloud Integration:** Save locked app configurations and logs to the cloud.
- **UI Improvements:** Modernize the interface with a cleaner design.

## Contributing

Contributions are welcome! Please submit a pull request or open an issue for discussion.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For support or inquiries, please contact [yourname@domain.com](mailto:yourname@domain.com).
