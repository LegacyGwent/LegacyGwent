# Gwent Trinket Award Script (Shell Version)

This bash script automates sending requests to award avatars, titles, and borders to users in the Gwent server.

## Prerequisites

- Bash shell (Linux, macOS, or Windows with WSL/Git Bash)
- `curl` command-line tool

## Installation

1. Make the script executable (Linux/macOS):
```bash
chmod +x award_trinkets.sh
```

2. On Windows with Git Bash or WSL, you can run it directly:
```bash
./award_trinkets.sh
```

## Usage

### Basic Usage
```bash
./award_trinkets.sh
```

### Custom Server URL
```bash
./award_trinkets.sh ./award_trinkets.sh http://localhost:5005
```

## Interactive Mode

The script runs in interactive mode and will prompt you for:

1. **Usernames**: Enter comma-separated usernames
   ```
   Enter usernames (comma-separated):
   > player1, player2, player3
   ```

2. **Trinket Type**: Select from the available options
   ```
   Select trinket type:
     1. Avatar
     2. Title
     3. Border
   > 1
   ```

3. **Trinket ID**: Enter the specific trinket ID
   ```
   Enter Avatar ID:
   > Phoenix
   ```

4. **Confirmation**: Review and confirm the request
   ```
   Proceed? (y/N): y
   ```

## Example Usage

### Awarding an Avatar
```
=== Gwent Trinket Award Tool ===

Enter usernames (comma-separated):
> GeraltPlayer, TrissFan, YenneferLover

Select trinket type:
  1. Avatar
  2. Title
  3. Border
> 1

Enter Avatar ID:
> Phoenix

📤 Sending request:
  Users: GeraltPlayer, TrissFan, YenneferLover
  Type: Avatar
  ID: Phoenix
  Endpoint: http://cynthia.ovyno.com:5005/api/GwentData/AwardTrinketToUsers

Proceed? (y/N): y

📤 Sending request...

=== Results ===
📊 Summary:
  Total Users: 3
  Success: 3
  Failed: 0
  Trinket Type: Avatar
  Trinket ID: Phoenix

📋 Individual Results:
  ✅ GeraltPlayer: Success
  ✅ TrissFan: Success
  ✅ YenneferLover: Success
```

## Features

### Color-coded Output
- ✅ Green: Success messages
- ❌ Red: Error messages
- ⚠️ Yellow: Warning messages
- 📤 Blue: Information messages

### Error Handling
- Validates curl availability
- Handles network timeouts
- Parses JSON responses
- Graceful error display

### Input Validation
- Validates usernames (removes empty entries)
- Validates trinket type selection
- Validates trinket ID input
- Confirms action before sending

## Common Trinket IDs

### Avatars
- `Phoenix` - Phoenix avatar
- `GeraltOfRivia` - Geralt avatar
- `TrissMerigold` - Triss avatar
- `Yennefer` - Yennefer avatar
- `NoAvatar` - Default avatar

### Titles
- `GOODGAMER` - Good Gamer title
- `CARDSMITH` - Card Smith title
- `OCCASIONALDRINKER` - Occasional Drinker title
- `$$$MILLIONAIRE$$$` - Millionaire title

### Borders
- `G_Phoenix` - Phoenix border
- `G_Beer` - Beer border
- `NoBorder` - Default border
- `Rank3border` - Rank 3 border
- `Rank6border` - Rank 6 border

## Technical Details

### Dependencies
- **curl**: For HTTP requests
- **bash**: Shell environment
- **sed/grep**: For text processing

### JSON Handling
The script manually constructs JSON payloads and parses JSON responses using:
- String manipulation for JSON construction
- `grep` and `sed` for JSON parsing
- Array handling for usernames

### Network Settings
- Connection timeout: 30 seconds
- Request timeout: 60 seconds
- Content-Type: application/json

## Notes

- The script defaults to the production server `http://cynthia.ovyno.com:5005`
- You can cancel the operation at any time with `Ctrl+C`
- The script validates input before sending requests
- Results show both summary statistics and individual user results
- Works on Linux, macOS, and Windows (with WSL/Git Bash)

## Troubleshooting

### "curl is not installed"
Install curl on your system:
- **Ubuntu/Debian**: `sudo apt-get install curl`
- **CentOS/RHEL**: `sudo yum install curl`
- **macOS**: `brew install curl`
- **Windows**: Download from https://curl.se/windows/

### Permission Denied
Make the script executable:
```bash
chmod +x award_trinkets.sh
```

### Network Issues
Check your internet connection and server availability:
```bash
curl -I http://cynthia.ovyno.com:5005
```
