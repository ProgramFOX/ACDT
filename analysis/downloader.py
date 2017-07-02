"""Downloads recent antichess games of a player to be investigated."""

from enum import Enum
import time

import bs4

class DownloadStatus(Enum):
    """Represents status after attempting to download games."""
    success = 0
    closed = 1
    noGames = 2
    tooManyRequests = 3


def game_ids_from_advanced_search(first_page, url, count_to_fetch, session):
    """Fetches game IDs from Advanced Search."""
    i = 1
    game_ids = []
    while True:
        if i == 1:
            page = first_page
        else:
            page = bs4.BeautifulSoup(session.get(url.format(i)).text,
                                     "html.parser")
        game_rows = page.select("div.game_row.paginated_element")
        for row in game_rows:
            game_ids.append(
                row.select_one("a.mini_board")["href"]
                .strip("/")
                .replace("/black", "")
                .replace("/white", "")
            )
            if len(game_ids) >= count_to_fetch:
                break
        if len(game_ids) >= count_to_fetch:
            break
        i += 1
    return game_ids


def download_one(user_id, session):
    """Download the games of a user and store them in the right directory
    depending on whether the games are reference (cheat or legit) or investigation."""
    base_url = "https://lichess.org/@/{}" \
               "/search?page={}" \
               "&clock.initMin={}" \
               "&clock.initMax={}" \
               "&perf=13&winner={}" \
               "&mode=1" \
               "&sort.field=d" \
               "&sort.order=desc" \
               "&turnsMin=5" \
               "&_={}"

    adv_search_1 = session.get(base_url.format(user_id, 1, 0, 60, user_id, time.time() * 1000))
    html_response = bs4.BeautifulSoup(adv_search_1.text, "html.parser")

    if "account is closed" in html_response.getText():
        return DownloadStatus.closed, None

    if "No game found" in html_response.getText():
        game_ids = []
    else:
        found_count = int(
            html_response.select_one(".search_status strong")
            .getText()
            .split(" ")[0]
            .replace(",", "")
        )
        count_to_fetch = min(found_count, 50)

        game_ids = game_ids_from_advanced_search(html_response,
                                                 base_url.format(user_id,
                                                                 "{}",
                                                                 0,
                                                                 60,
                                                                 user_id,
                                                                 time.time() * 1000),
                                                 count_to_fetch,
                                                 session)

    adv_search_2 = session.get(base_url.format(user_id, 1, 120, 10800, user_id, time.time() * 1000))
    html_response = bs4.BeautifulSoup(adv_search_2.text, "html.parser")

    if "account is closed" in html_response.getText():
        return DownloadStatus.closed, None
    if "No game found" in html_response.getText() and not game_ids:
        return DownloadStatus.noGames, None
    elif "No game found" in html_response.getText() and found_count != 0:
        pass
    else:
        found_count = int(
            html_response.select_one(".search_status strong")
            .getText()
            .split(" ")[0]
            .replace(",", "")
        )
        count_to_fetch = min(found_count, 50)
        game_ids += game_ids_from_advanced_search(html_response,
                                                  base_url.format(user_id,
                                                                  "{}",
                                                                  120,
                                                                  10800,
                                                                  user_id,
                                                                  time.time() * 1000),
                                                  count_to_fetch,
                                                  session)

    games = session.post("https://lichess.org/api/games?with_moves=1", data=",".join(game_ids)).text
    if "Try again later" in games:
        return DownloadStatus.tooManyRequests, None

    return DownloadStatus.success, games
