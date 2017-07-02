"""This script deals with the investigation queue that gets
filled when moderators request analysis through the web UI."""

from enum import Enum
import json
import sys
import time
import urllib.parse

import requests

import downloader
import engine_check
from cancellationtoken import CancellationToken

class ApiResponseStatus(Enum):
    """An enum that represents possible statuses after doing an API call."""
    success = 0
    no_content = 1
    http_error = 2

def load_api_key():
    """Loads the web API key from secret/apikey.txt"""
    with open("analysis/secret/apikey.txt", "r") as file_object:
        return file_object.read().strip()

def investigation_from_analysis(engine_check_result, rating, game_created_date,
                                speed, player, game_id):
    """Takes the result of engine_check.analyze_one_game, a few more parameters,
    and returns an object following the Investigation model (see web)."""
    sjeng_percentage, sf_percentage, sjeng_full, sf_full = engine_check_result
    max_percentage = max(sjeng_percentage, sf_percentage)
    return {
        "max": max_percentage,
        "sjengFull": sjeng_full,
        "sjeng": sjeng_percentage,
        "stockfishFull": sf_full,
        "stockfish": sf_percentage,
        "rating": rating,
        "gameCreatedDate": game_created_date,
        "speed": speed,
        "player": player,
        "gameId": game_id
    }

def investigate_one_player(user_id, session, stockfish, sf_info_handler):
    """Investigates one player and pushes the result to the server."""
    download_status, downloaded_games = downloader.download_one(user_id, session)
    investigations = []
    if download_status == downloader.DownloadStatus.success:
        games = json.loads(downloaded_games)
        for game in games:
            moves = game["turns"].split(" ")
            analyze_white = game["players"]["white"]["userId"] == user_id

            game_created_date = game["createdAt"]
            game_speed = game["speed"]
            user_rating = game["players"]["white" if analyze_white else "black"]["rating"]
            game_id = game["id"]

            ecr = engine_check.analyze_one_game(moves, analyze_white, stockfish, sf_info_handler)
            investigations.append(
                investigation_from_analysis(ecr,
                                            user_rating,
                                            game_created_date,
                                            game_speed,
                                            user_id,
                                            game_id)
            )
    return investigations

def next_queue_item(session, api_key, base_url):
    """Gives the next player name from the investigation queue."""
    url = urllib.parse.urljoin(base_url, "/Api/QueueNext")
    response = session.post(url, data={"key": api_key})
    if response.status_code == 200:
        name = response.text.strip()
        if name != "":
            return ApiResponseStatus.success, name
        else:
            return ApiResponseStatus.no_content, None
    else:
        return ApiResponseStatus.http_error, response.status_code

def set_queue_item_as_in_progress(session, api_key, base_url, player_name):
    """Tells the web API that the script is about to process a certan player."""
    url = urllib.parse.urljoin(base_url, "/Api/QueueItemInProgress")
    response = session.post(url, data={"key": api_key, "playerName": player_name})
    if response.status_code == 200:
        return ApiResponseStatus.success, 200
    else:
        return ApiResponseStatus.http_error, response.status_code

def push_investigation_result(session, api_key, base_url, player_name, investigation_result):
    """Pushes the result of an investigation to the server."""
    url = urllib.parse.urljoin(base_url, "/Api/PlayerGamesProcessed")
    response = session.post(url, data={
        "key": api_key, "playerName": player_name, "games": investigation_result
    })
    if response.status_code == 200:
        return ApiResponseStatus.success, 200
    else:
        return ApiResponseStatus.http_error, response.status_code

def handle_queue(args, cancellation_token):
    """Function where the 'real work', the handling of the investigation queue, happens."""
    base_url = args[1]

    session = requests.Session()
    api_key = load_api_key()

    stockfish, sf_info_handler = engine_check.create_stockfish_instance()

    while not cancellation_token.is_cancellation_requested():
        next_fetched, next_player_or_http_result = next_queue_item(session, api_key, base_url)
        if next_fetched == ApiResponseStatus.no_content:
            time.sleep(120)
            continue
        elif next_fetched == ApiResponseStatus.http_error:
            print("ERROR: Status code " + str(next_player_or_http_result) + ". Aborting.")
            break
        next_player = next_player_or_http_result
        print("Working on: " + next_player)
        set_queue_item_as_in_progress(session, api_key, base_url, next_player)
        result = investigate_one_player(next_player, session, stockfish, sf_info_handler)
        push_investigation_result(session, api_key, base_url, next_player, result)
        print("Finished: " + next_player)

def main():
    """Calls handle_queue and deals with cancellation when Ctrl-C is pressed."""
    cancellation_token = CancellationToken()
    handle_queue(sys.argv, cancellation_token)

if __name__ == "__main__":
    main()
