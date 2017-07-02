"""This module holds the CancellationToken class."""

class CancellationToken:
    """Based on .NET's CancellationToken class. To be used to tell to cancel an operation."""
    def __init__(self):
        self._cancellation_requested = False

    def request_cancellation(self):
        """Requests cancellation on an operation."""
        self._cancellation_requested = True

    def is_cancellation_requested(self):
        """An operation can check here if it should be cancelled."""
        return self._cancellation_requested
