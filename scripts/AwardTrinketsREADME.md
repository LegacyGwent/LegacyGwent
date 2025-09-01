# Gwent Trinket Award Script

This Python script automates sending requests to award avatars, titles, and borders to users in the Gwent server.

## Prerequisites

- Python 3.6 or higher
- `requests` library

## Installation

1. Install the required dependencies:
```bash
pip install -r requirements.txt
```

## Usage

### Basic Usage
```bash
python award_trinkets.py
```

### Custom Server URL
```bash
python award_trinkets.py http://your-server:5005
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

🔄 Sending request...

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

## Error Handling

The script handles various error scenarios:
- Network connection issues
- Invalid server responses
- Missing or invalid input
- Server errors

## Notes

- The script defaults to the production server `http://cynthia.ovyno.com:5005` if no server URL is provided
- You can cancel the operation at any time with `Ctrl+C`
- The script validates input before sending requests
- Results show both summary statistics and individual user results
