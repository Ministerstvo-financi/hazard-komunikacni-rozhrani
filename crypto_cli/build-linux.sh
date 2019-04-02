#!/bin/sh

DIR=$(dirname $0)
(
cd $DIR
mvn  -DskipTests clean install
)