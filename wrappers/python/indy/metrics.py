from .libindy import do_call, create_cb

from ctypes import *

import logging


async def collect_metrics() -> bool:
    """
    Collect metrics from libindy.

    :return: String with a dictionary of metrics in JSON format. Where keys are names of metrics.
    """

    logger = logging.getLogger(__name__)
    logger.debug("collect_metrics: >>>")

    if not hasattr(collect_metrics, "cb"):
        logger.debug("collect_metrics: Creating callback")
        collect_metrics.cb = create_cb(CFUNCTYPE(None, c_int32, c_int32, c_char_p))

    res = await do_call('indy_collect_metrics',
                        collect_metrics.cb)

    logger.debug("collect_metrics: <<< res: %r", res)
    return res
