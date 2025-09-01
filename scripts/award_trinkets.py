#!/usr/bin/env python3
"""
Gwent Trinket Award Script
Automates sending requests to award avatars, titles, and borders to users.
"""

import requests
import json
import sys
from typing import List, Dict, Any

class GwentTrinketAwarder:
    def __init__(self, base_url: str = "http://cynthia.ovyno.com:5005"):
        self.base_url = base_url
        self.endpoint = f"{base_url}/api/GwentData/AwardTrinketToUsers"
        
        # Trinket type mapping
        self.trinket_types = {
            "1": "Avatar",
            "2": "Title", 
            "3": "Border"
        }
    
    def get_user_input(self) -> Dict[str, Any]:
        """Get user input for the trinket award request."""
        print("=== Gwent Trinket Award Tool ===\n")
        
        # Get usernames
        print("Enter usernames (comma-separated):")
        usernames_input = input("> ").strip()
        usernames = [name.strip() for name in usernames_input.split(",") if name.strip()]
        
        if not usernames:
            print("Error: No valid usernames provided.")
            return None
        
        # Get trinket type
        print("\nSelect trinket type:")
        for key, value in self.trinket_types.items():
            print(f"  {key}. {value}")
        
        trinket_type_choice = input("> ").strip()
        if trinket_type_choice not in self.trinket_types:
            print("Error: Invalid trinket type selection.")
            return None
        
        # Convert to enum value (0-based index)
        trinket_type_enum = int(trinket_type_choice) - 1
        
        # Get trinket ID
        print(f"\nEnter {self.trinket_types[trinket_type_choice]} ID:")
        trinket_id = input("> ").strip()
        
        if not trinket_id:
            print("Error: No trinket ID provided.")
            return None
        
        return {
            "usernames": usernames,
            "trinketType": trinket_type_enum,
            "trinketId": trinket_id
        }
    
    def send_request(self, request_data: Dict[str, Any]) -> Dict[str, Any]:
        """Send the request to the server."""
        try:
            headers = {
                "Content-Type": "application/json"
            }
            
            response = requests.post(
                self.endpoint,
                json=request_data,
                headers=headers,
                timeout=30
            )
            
            if response.status_code == 200:
                return response.json()
            else:
                return {
                    "error": True,
                    "status_code": response.status_code,
                    "message": response.text
                }
                
        except requests.exceptions.RequestException as e:
            return {
                "error": True,
                "message": f"Request failed: {str(e)}"
            }
    
    def display_results(self, results: Dict[str, Any]) -> None:
        """Display the results in a formatted way."""
        print("\n=== Results ===")
        
        if results.get("error"):
            print(f"❌ Error: {results.get('message', 'Unknown error')}")
            if results.get("status_code"):
                print(f"Status Code: {results['status_code']}")
            return
        
        # Display summary
        print(f"📊 Summary:")
        print(f"  Total Users: {results.get('totalUsers', 0)}")
        print(f"  Success: {results.get('successCount', 0)}")
        print(f"  Failed: {results.get('failureCount', 0)}")
        print(f"  Trinket Type: {results.get('trinketType', 'Unknown')}")
        print(f"  Trinket ID: {results.get('trinketId', 'Unknown')}")
        
        # Display individual results
        if results.get('results'):
            print(f"\n📋 Individual Results:")
            for result in results['results']:
                username = result.get('username', 'Unknown')
                status = result.get('status', 'Unknown')
                
                if status == "Success":
                    print(f"  ✅ {username}: Success")
                elif status == "Failed":
                    print(f"  ❌ {username}: Failed")
                else:
                    message = result.get('message', 'Unknown error')
                    print(f"  ⚠️  {username}: Error - {message}")
    
    def run(self) -> None:
        """Main execution method."""
        try:
            # Get user input
            request_data = self.get_user_input()
            if not request_data:
                return
            
            # Confirm before sending
            print(f"\n📤 Sending request:")
            print(f"  Users: {', '.join(request_data['usernames'])}")
            print(f"  Type: {self.trinket_types[str(request_data['trinketType'] + 1)]}")
            print(f"  ID: {request_data['trinketId']}")
            print(f"  Endpoint: {self.endpoint}")
            
            confirm = input("\nProceed? (y/N): ").strip().lower()
            if confirm not in ['y', 'yes']:
                print("Request cancelled.")
                return
            
            # Send request
            print("\n🔄 Sending request...")
            results = self.send_request(request_data)
            
            # Display results
            self.display_results(results)
            
        except KeyboardInterrupt:
            print("\n\nOperation cancelled by user.")
        except Exception as e:
            print(f"\n❌ Unexpected error: {str(e)}")

def main():
    """Main entry point."""
    # Default to production server
    base_url = "http://cynthia.ovyno.com:5005"
    
    # Allow command line override
    if len(sys.argv) > 1:
        base_url = sys.argv[1]
    
    awarder = GwentTrinketAwarder(base_url)
    awarder.run()

if __name__ == "__main__":
    main()
