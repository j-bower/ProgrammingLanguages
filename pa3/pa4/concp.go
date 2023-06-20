package main

import (
	"fmt"
	"io"
	"os"
	"path/filepath"
	"sync"
)

func main() {
	files := os.Args[1 : len(os.Args)-1]
	dst := os.Args[len(os.Args)-1]

	if len(os.Args) < 3 {
		fmt.Printf("usage: ./concp file1 file2....dest\n")
		os.Exit(1)
	}

	inf, err := os.Stat(dst)
	if err != nil {
		fmt.Printf("%v\n", err)
		os.Exit(1)
	} else if !inf.IsDir() {
		fmt.Printf("Dst is not a directory\n")
		os.Exit(1)
	}

	var wg sync.WaitGroup
	errors := make(chan error, len(files))

	for _, file := range files {
		wg.Add(1)
		go func(toCopy, dst string) {
			defer wg.Done()
			err := fileCopy(toCopy, filepath.Join(dst, filepath.Base(toCopy)))
			if err != nil {
				errors <- fmt.Errorf("Failed to copy %s: %w", toCopy, err)
			}
		}(file, dst)
	}

	go func() {
		wg.Wait()
		close(errors)
	}()

	var errs []error
	for err := range errors {
		if err != nil {
			errs = append(errs, err)
		}
	}

	if len(errs) > 0 {
		for _, err := range errs {
			fmt.Printf("error: %v\n", err)
		}
		os.Exit(1)
	}
}

func fileCopy(toCopy, dst string) error {
	info, err := os.Stat(toCopy)

	// error check the file
	if err != nil {
		return err
	} else if info.IsDir() {
		return fmt.Errorf("Not a regualr file: %s", toCopy)
	}

	// open the file
	fileToCopy, err := os.Open(toCopy)
	if err != nil {
		return err
	}
	defer fileToCopy.Close()

	// create the file
	fileDst, err := os.Create(dst)
	if err != nil {
		return err
	}
	defer fileDst.Close()

	// read the file-to-copy
	buffer := make([]byte, 32)
	for {
		n, err := fileToCopy.Read(buffer)
		if err != nil {
			if err == io.EOF {
				break
			}
			return err
		}
		if _, err := fileDst.Write(buffer[:n]); err != nil {
			return err
		}
	}
	return nil
}
