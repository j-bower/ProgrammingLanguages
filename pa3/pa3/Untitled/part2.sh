#!/usr/bin/env bash

# Your solution for part 2 goes here.
i=1 maxHp=0
while read -r line; do
    # skip first line
    test $i -eq 1 && ((i=i+1)) && continue

    # get the make and horsepower
    make=$(echo $line | cut -d, -f1)
    hp=$(echo $line | cut -d, -f6)

    # compare hp values
    if [ $(echo "$hp > $maxHp" | bc) == 1 ]; then
        maxHp=$hp
        maxMake=$make
    fi
    
done < cars.csv
echo $maxMake $maxHp > answer.txt