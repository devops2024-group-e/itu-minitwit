#!/bin/bash

# Get server categories
echo "Getting categories from Pulumi output"
declare -a categories=($(pulumi stack output --json | jq -r 'keys | .[]'))

# If file exists then remove it
if [ -e "inventory.ansible" ]; then
	rm inventory.ansible
fi

echo "Adding all server category..."
# Create the ansible inventory file with the all category
touch inventory.ansible
echo "[all]" >> inventory.ansible

# Concatenates the arrays of ips from the different categories of servers and outputs
# them to inventory file for the 'all' category
pulumi stack output --json | jq -r 'add | .[]' >> inventory.ansible

echo "" >> inventory.ansible # Add space between the last entry in the all category and the next category

# Go through each server category and creates it as an entry in the
for cate in "${categories[@]}"; do
	echo "Adding ${cate} server category..."
	echo "[${cate}]" >> inventory.ansible
	pulumi stack output --json | jq -r --arg category "${cate}" '.[$category] | .[]' >> inventory.ansible

	echo "" >> inventory.ansible
done

echo "Ansible inventory file generated"
