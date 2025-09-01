#!/bin/bash

# Gwent Trinket Award Script (Shell Version)
# Automates sending requests to award avatars, titles, and borders to users.

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default server URL
DEFAULT_SERVER="http://cynthia.ovyno.com:5005"

# Function to print colored output
print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_info() {
    echo -e "${BLUE}📤 $1${NC}"
}

# Function to check if curl is available
check_curl() {
    if ! command -v curl &> /dev/null; then
        print_error "curl is not installed. Please install curl to use this script."
        exit 1
    fi
}

# Function to get user input
get_user_input() {
    echo "=== Gwent Trinket Award Tool ==="
    echo ""

    # Get usernames
    echo "Enter usernames (comma-separated):"
    read -p "> " usernames_input
    
    # Clean up usernames
    usernames=$(echo "$usernames_input" | tr ',' '\n' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//' | grep -v '^$' | tr '\n' ',' | sed 's/,$//')
    
    if [ -z "$usernames" ]; then
        print_error "No valid usernames provided."
        return 1
    fi

    # Get trinket type
    echo ""
    echo "Select trinket type:"
    echo "  1. Avatar"
    echo "  2. Title"
    echo "  3. Border"
    read -p "> " trinket_type_choice

    case $trinket_type_choice in
        1) trinket_type="Avatar"; trinket_type_enum=0 ;;
        2) trinket_type="Title"; trinket_type_enum=1 ;;
        3) trinket_type="Border"; trinket_type_enum=2 ;;
        *) print_error "Invalid trinket type selection."; return 1 ;;
    esac

    # Get trinket ID
    echo ""
    echo "Enter $trinket_type ID:"
    read -p "> " trinket_id

    if [ -z "$trinket_id" ]; then
        print_error "No trinket ID provided."
        return 1
    fi

    return 0
}

# Function to send request
send_request() {
    local server_url="$1"
    local usernames="$2"
    local trinket_type_enum="$3"
    local trinket_id="$4"
    
    # Convert comma-separated usernames to JSON array
    local json_usernames="["
    IFS=',' read -ra USER_ARRAY <<< "$usernames"
    for i in "${!USER_ARRAY[@]}"; do
        if [ $i -gt 0 ]; then
            json_usernames+=","
        fi
        json_usernames+="\"${USER_ARRAY[i]}\""
    done
    json_usernames+="]"

    # Create JSON payload
    local json_payload="{
        \"usernames\": $json_usernames,
        \"trinketType\": $trinket_type_enum,
        \"trinketId\": \"$trinket_id\"
    }"

    # Send request
    local response=$(curl -s -w "\n%{http_code}" \
        -X POST \
        -H "Content-Type: application/json" \
        -d "$json_payload" \
        --connect-timeout 30 \
        --max-time 60 \
        "$server_url/api/GwentData/AwardTrinketToUsers")

    # Extract status code and response body
    local http_code=$(echo "$response" | tail -n1)
    local response_body=$(echo "$response" | head -n -1)

    if [ "$http_code" = "200" ]; then
        echo "$response_body"
    else
        echo "{\"error\": true, \"status_code\": $http_code, \"message\": \"$response_body\"}"
    fi
}

# Function to display results
display_results() {
    local results="$1"
    
    echo ""
    echo "=== Results ==="
    
    # Check if response contains error
    if echo "$results" | grep -q '"error":\s*true'; then
        local error_msg=$(echo "$results" | grep -o '"message":\s*"[^"]*"' | cut -d'"' -f4)
        local status_code=$(echo "$results" | grep -o '"status_code":\s*[0-9]*' | cut -d':' -f2 | tr -d ' ')
        print_error "Error: ${error_msg:-Unknown error}"
        if [ -n "$status_code" ]; then
            echo "Status Code: $status_code"
        fi
        return
    fi

    # Extract values from JSON response
    local total_users=$(echo "$results" | grep -o '"totalUsers":\s*[0-9]*' | cut -d':' -f2 | tr -d ' ')
    local success_count=$(echo "$results" | grep -o '"successCount":\s*[0-9]*' | cut -d':' -f2 | tr -d ' ')
    local failure_count=$(echo "$results" | grep -o '"failureCount":\s*[0-9]*' | cut -d':' -f2 | tr -d ' ')
    local trinket_type=$(echo "$results" | grep -o '"trinketType":\s*"[^"]*"' | cut -d'"' -f4)
    local trinket_id=$(echo "$results" | grep -o '"trinketId":\s*"[^"]*"' | cut -d'"' -f4)

    # Display summary
    echo "📊 Summary:"
    echo "  Total Users: ${total_users:-0}"
    echo "  Success: ${success_count:-0}"
    echo "  Failed: ${failure_count:-0}"
    echo "  Trinket Type: ${trinket_type:-Unknown}"
    echo "  Trinket ID: ${trinket_id:-Unknown}"

    # Display individual results
    if echo "$results" | grep -q '"results":'; then
        echo ""
        echo "📋 Individual Results:"
        
        # Extract individual results
        local results_section=$(echo "$results" | sed -n '/"results":\s*\[/,/\]/p')
        
        # Parse each result entry
        echo "$results_section" | grep -o '"[^"]*":\s*"[^"]*"' | while IFS=':' read -r key value; do
            local clean_key=$(echo "$key" | tr -d '"')
            local clean_value=$(echo "$value" | tr -d '"')
            
            case $clean_key in
                "username")
                    current_username="$clean_value"
                    ;;
                "status")
                    case $clean_value in
                        "Success")
                            echo "  ✅ $current_username: Success"
                            ;;
                        "Failed")
                            echo "  ❌ $current_username: Failed"
                            ;;
                        *)
                            echo "  ⚠️  $current_username: $clean_value"
                            ;;
                    esac
                    ;;
            esac
        done
    fi
}

# Function to confirm action
confirm_action() {
    local usernames="$1"
    local trinket_type="$2"
    local trinket_id="$3"
    local server_url="$4"
    
    echo ""
    print_info "Sending request:"
    echo "  Users: $usernames"
    echo "  Type: $trinket_type"
    echo "  ID: $trinket_id"
    echo "  Endpoint: $server_url/api/GwentData/AwardTrinketToUsers"
    echo ""
    read -p "Proceed? (y/N): " confirm
    
    case $confirm in
        [Yy]|[Yy][Ee][Ss]) return 0 ;;
        *) return 1 ;;
    esac
}

# Main function
main() {
    # Check for curl
    check_curl
    
    # Get server URL from command line or use default
    local server_url="${1:-$DEFAULT_SERVER}"
    
    # Get user input
    if ! get_user_input; then
        exit 1
    fi
    
    # Confirm action
    if ! confirm_action "$usernames" "$trinket_type" "$trinket_id" "$server_url"; then
        print_warning "Request cancelled."
        exit 0
    fi
    
    # Send request
    echo ""
    print_info "Sending request..."
    local results=$(send_request "$server_url" "$usernames" "$trinket_type_enum" "$trinket_id")
    
    # Display results
    display_results "$results"
}

# Handle script interruption
trap 'echo ""; print_warning "Operation cancelled by user."; exit 1' INT

# Run main function
main "$@"
