#!/usr/bin/env bash

# Your solution for part 3 goes here.
echo $(awk -F',' '$6 > max && NR>1 { max=$6; make=$1} END {printf("%s %.6f\n", make, max) }' < cars.csv) > answer.txt