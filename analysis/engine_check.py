"""This module contains functions to compare an antichess engine with Stockfish and Sjeng."""

import os
import os.path
import subprocess

import chess
import chess.uci
import chess.variant

from fishnet.fishnet import stockfish_command


def get_sjeng_move(previous_moves):
    """Get Sjeng's recommended move based on a list of all previous moves."""
    sj_path = r"C:\Program Files (x86)\Suicide Challenger\SjengS.exe"
    subp = subprocess.Popen([sj_path],
                            stdin=subprocess.PIPE,
                            stdout=subprocess.PIPE,
                            cwd=os.path.dirname(sj_path))

    for command in [b"new", b"variant suicide", b"difficulty 200000", b"force"] +\
            list(map(str.encode, previous_moves))\
            + [b"go"]:
        subp.stdin.write(command + str.encode(os.linesep))
        subp.stdin.flush()
    subp.stdin.close()
    while True:
        line_bytes = subp.stdout.readline()
        line = bytes.decode(line_bytes)
        stripped = line.strip()
        if len(stripped) == 4 or len(stripped) == 5:
            subp.stdout.close()
            subp.kill()
            return stripped


def analyze_one_game(moves, analyze_white, sf_engine, sf_info_handler):
    """Analyzes one antichess game."""
    board = chess.variant.GiveawayBoard()
    whites_turn = True

    sjeng_comparison = []
    sf_comparison = []

    counted_for_percentage = 0
    sjeng_for_percentage = 0
    sf_for_percentage = 0

    uci_moves = []

    sf_engine.ucinewgame()

    for move in moves:
        if whites_turn == analyze_white:
            if len(board.legal_moves) > 1:
                counted_for_percentage += 1
            else:
                sjeng_comparison.append(-1)
                sf_comparison.append(-1)
                board.push_san(move)
                uci_moves.append(board.move_stack[-1].uci())
                continue

            fen = board.fen()
            played_move = board.push_san(move).uci()

            sjeng_move = get_sjeng_move(uci_moves)

            uci_moves.append(board.move_stack[-1].uci())

            if sjeng_move == played_move:
                sjeng_for_percentage += 1
                sjeng_comparison.append(1)
            else:
                sjeng_comparison.append(0)

            sf_engine.position(chess.variant.GiveawayBoard(fen))
            sf_engine.go(depth=18)
            sf_move = sf_info_handler.info["pv"][1][0].uci()
            if sf_move == played_move:
                sf_for_percentage += 1
                sf_comparison.append(1)
            else:
                sf_comparison.append(0)
        else:
            board.push_san(move)
            uci_moves.append(board.move_stack[-1].uci())
        whites_turn = not whites_turn

    return sjeng_for_percentage / counted_for_percentage,\
        sf_for_percentage / counted_for_percentage,\
        sjeng_comparison, sf_comparison


def create_stockfish_instance():
    """Creates an instance of the Stockfish engine and an info handler."""
    sf_engine = chess.uci.popen_engine(stockfish_command())
    sf_engine.setoption({'Threads': 4, 'Hash': 2048, 'UCI_Variant': 'giveaway'})
    sf_engine.uci()
    info_handler = chess.uci.InfoHandler()
    sf_engine.info_handlers.append(info_handler)
    return sf_engine, info_handler
