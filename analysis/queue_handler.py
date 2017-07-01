"""This script deals with the investigation queue that gets
filled when moderators request analysis through the web UI."""

import json

import requests

import downloader
import engine_check

def load_api_key():
    """Loads the web API key from secret/apikey.txt"""
    with open("analysis/secret/apikey.txt", "r") as file_object:
        return file_object.read().strip()

def investigate_one_player(user_id, session, stockfish, sf_info_handler):
    """Investigates one player and pushes the result to the server."""
    download_status, downloaded_games = downloader.download_one(user_id, session)
    if download_status == downloader.DownloadStatus.success:
        games = json.loads(downloaded_games)
        for game in games:
            moves = game["turns"].split(" ")
            analyze_white = game["players"]["white"]["userId"] == user_id

            engine_check.analyze_one_game(moves, analyze_white, stockfish, sf_info_handler)

def main():
    """Main method of script: where the 'real work' happens."""
    session = requests.Session()
    api_key = load_api_key()

    stockfish, sf_info_handler = engine_check.create_stockfish_instance()

if __name__ == "__main__":
    main()
