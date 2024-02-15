"""
    MiniTwit
    ~~~~~~~~

    A microblogging application written with Flask and sqlite3.

    :copyright: (c) 2010 by Armin Ronacher.
    :license: BSD, see LICENSE for more details.
"""
from __future__ import with_statement
import re
import time
import sqlite3
from hashlib import md5
from datetime import datetime
from contextlib import closing
from flask import Flask, request, session, url_for, redirect, \
     render_template, abort, g, flash
from werkzeug.security import generate_password_hash, check_password_hash


# configuration
DATABASE = './test-tmp/minitwit.db'
PER_PAGE = 30
DEBUG = True
SECRET_KEY = 'development key'

# create our little application :)
app = Flask(__name__)


def connect_db():
    """Returns a new connection to the database."""
    return sqlite3.connect(DATABASE)


def init_db():
    """Creates the database tables."""
    with closing(connect_db()) as db:
        with app.open_resource('schema.sql') as f:
            db.cursor().executescript(f.read().decode('UTF-8'))
        db.commit()
