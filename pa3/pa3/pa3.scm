(define (list-len l)
    #f ;; TODO: return something other than FALSE
    )

(define (inc-list n)
    #f ;; TODO: return something other than FALSE
    )

(define (rev-list l)
    #f ;; TODO: return something other than FALSE
    )

(define (my-map f l)
    #f ;; TODO: return something other than FALSE
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

    #f ;; TODO: return something other than FALSE
    )
