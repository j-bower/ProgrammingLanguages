#!/usr/bin/env bash

# Your solution for part 4 goes here.

# create sequence.txt
awk '/sequence position/ { pos=$3; } /sequence data:/ { seq=$3; printf("%d,%s\n", pos, seq)}' *.sample | sort -n | cut -d, -f2 | tr -d '\n' > sequence.txt

# fold to get each base pair on own line
fold -w1 sequence.txt > folds.txt

# make pairs.txt
awk '
    { pairs[$1]++ }
    END {
        printf("%s %d\n", "A", pairs["A"]);
        printf("%s %d\n", "T", pairs["T"]);
        printf("%s %d\n", "C", pairs["C"]);
        printf("%s %d\n", "G", pairs["G"]);
    }
' < folds.txt > pairs.txt

# get every three charatcers on own line
fold -w3 sequence.txt > folds.txt

# create codons.txt
awk '
    { codons[$1]++; total++ }
    END {
        for (key in codons) {
            printf("%s %.6f%\n", key, codons[key]*100/total)
        }
    }
' < folds.txt | sort > codons.txt

# remove tmp file
rm folds.txt