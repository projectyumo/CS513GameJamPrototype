#!/usr/bin/env python3

import pandas as pd
import matplotlib.pyplot as plt
import sys

# Global constant for selected levels
SELECTED_LEVELS = [3, 4, 5, 6, 7, 9, 10]


def plot_glass_shelf_data(json_file_path):
    # Load the data
    data = pd.read_json(json_file_path)

    # Transpose and convert columns to numeric
    data_t = data.T
    data_t['currentLevel'] = pd.to_numeric(data_t['currentLevel'])
    data_t['glassShelfCollisions'] = pd.to_numeric(
        data_t['glassShelfCollisions'])
    data_t['glassShelfPassthroughs'] = pd.to_numeric(
        data_t['glassShelfPassthroughs'])

    # Group by level and sum the values
    grouped_data = data_t.groupby('currentLevel').sum()

    # Selecting specific levels
    grouped_selected_levels = grouped_data.loc[SELECTED_LEVELS]

    # Plotting
    plt.figure(figsize=(12, 6))
    plt.bar(grouped_selected_levels.index, grouped_selected_levels['glassShelfCollisions'],
            label='Glass Shelf Collisions')
    plt.bar(grouped_selected_levels.index, grouped_selected_levels['glassShelfPassthroughs'],
            bottom=grouped_selected_levels['glassShelfCollisions'], label='Glass Shelf Passthroughs')

    plt.xlabel('Level')
    plt.ylabel('Counts')
    plt.title('Glass Shelf Collisions vs Passthroughs for Selected Levels')
    plt.xticks(SELECTED_LEVELS)
    plt.legend()

    # Show the plot
    plt.show()


if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <path_to_json_file>")
        sys.exit(1)

    json_file_path = sys.argv[1]
    plot_glass_shelf_data(json_file_path)
