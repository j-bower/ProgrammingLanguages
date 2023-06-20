(define (list-len l)
    (if (null? l)
        0
        (+ 1 (list-len (cdr l))))
    )

(define (inc-list n)
    (if (= n 0)
        '()
        (append (inc-list (- n 1)) (list n)))
    )

(define (rev-list l)
    (if (null? l)
        '()
        (append (rev-list (cdr l)) (cons (car l) '())))
    )

(define (my-map f l)
    (if (null? l)
        '()
        (cons (f (car l)) (my-map f (cdr l))))
    )

(define (merge-sort l)
    ;; Split a list into two halves, returned in a pair. You may uncomment this.
    (define (split l)
        (define (split-rec pair)
            (let ((front (car pair)) (back (cdr pair)))
                (if (>= (length front) (length back))
                    pair
                    (split-rec (cons (append front (list (car back))) (cdr back))))))
        (split-rec (cons (list (car l)) (cdr l))))

    (define (merge first second)
        (if (null? first) 
            second
            (if (null? second) 
                first
                (if (< (car first) (car second)) 
                    (cons (car first) (merge (cdr first) second))
                    (cons (car second) (merge first (cdr second)))
                )
            )
        )
    )

    (if (null? l)
        '()
        (if (= (length l) 1)
            l
            (let ((s (split l)))
                (let ((a (car s)) (b (cdr s)))
                    (merge (merge-sort a) (merge-sort b))
                )
            )
        )
    )
)