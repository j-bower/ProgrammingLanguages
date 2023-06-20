#!/usr/bin/env bash

# Your solution for part 1 goes here.
for file in *.txt; do
    
    # remove file extension
    FILE=$(basename $file .txt)
    #echo $FILE
    
    # extract info from file
    idNum=$(awk '/id/ {  print $2; }' < $file)
    age=$(awk '/age/ { print $2; }' < $file)
    email=$(awk '/email/ { print $2; }' < $file)

    # create directory
    mkdir $FILE

    # add contents to directory
    echo $idNum > ${FILE}/id.txt
    echo $age > ${FILE}/age.txt
    echo $email > ${FILE}/email.txt

    # delete old .txt (file)
    rm $file
done