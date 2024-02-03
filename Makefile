init:
	python3 -c "from minitwit import init_db; init_db()"

build:
	gcc flag_tool.c -l sqlite3 -o flag_tool

clean:
	rm flag_tool

test:
	python3 minitwit_tests.py

start:
	sh ./control.sh start

stop:
	sh ./control.sh stop
