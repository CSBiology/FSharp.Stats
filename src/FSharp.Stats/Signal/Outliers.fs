﻿namespace FSharp.Stats.Signal 


module Outliers =
    open FSharp.Stats
    open Matrix

    /// Tukey's fences based on interquartile range. c defines the magnitude of interquartile range that is added/subtracted to Q3 and Q1 respectively.
    /// Commonly c is 1.5 for outliers and 3 for points 'far out' (Tukey 1977).
    let tukey (k:float) (d:float []) =
        let firstQ = Quantile.compute 0.25 d
        let thirdQ = Quantile.compute 0.75 d 
        let iqr = System.Math.Abs (thirdQ - firstQ)
        Intervals.create (firstQ - k * iqr) (thirdQ + k * iqr) 
        
        
    /// Returns Z Score for an individual point. 
    /// x - raw score(raw data)
    /// m - mean of the population
    /// s - standard deviation of the population
    let zScore (x:float) (m:float) (s:float) =
        (x - m) / s

    ///Returns a list of Z scores of a population
    let zScoresOfPopulation (ls:list<float>) =
        let m = List.mean ls
        let s = stDevPopulation(ls)
        [for x in ls -> zScore x m s]


    ///Returns a population interval according to desired max and min Z Score values    
    let populationIntervalByZScore (ls:list<float>) (minZ:float) (maxZ:float) =
        let m = List.mean ls
        let s = stDevPopulation(ls)
        Intervals.create (minZ * s + m) (maxZ * s + m)
    
    ///Returns a list of Z scores of a sample
    let zScoresOfSample (ls:list<float>) =
        let m = List.mean ls
        let s = stDev(ls)
        [for x in ls -> zScore x m s]

    ///Returns a sample interval according to desired max and min Z Score values    
    let sampleIntervalByZscore (ls:list<float>) (minZ:float) (maxZ:float) =
        let m = List.mean ls
        let s = stDev(ls)
        Intervals.create (minZ * s + m) (maxZ * s + m)

    ///Returns Mahalanobi's distance  for an individual observation in a matrix.
    ///dataSource - Sample or Population.
    ///orientation - RowWise or ColWise. (RowWise orientation means that each row is a Vector) 
    let mahalanobisDistance (observation:vector) (dataMatrix:matrix) (dataSource:DataSource) (orientation:Orientation) :float =
        
        let invertedCovarianceMatrix = Algebra.LinearAlgebra.Inverse(covarianceMatrixOf dataMatrix dataSource (orientation.Inverse))
        let meanVector = Matrix.mean dataMatrix (orientation.Inverse)
        let subObsMean = Matrix.ofVector(Vector.sub observation meanVector)
        let multObsCov = Matrix.mul (subObsMean.Transpose) invertedCovarianceMatrix
        let distance = Matrix.toScalar(Matrix.mul multObsCov subObsMean)
        sqrt distance
        
    ///Returns Mahalanobi's distance  for for every observation in a matrix.
    ///dataSource - Sample or Population.
    ///orientation - RowWise or ColWise. (RowWise orientation means that each row is a Vector) 
    let mahalanobisDistanceList (dataMatrix:matrix) (dataSource:DataSource) (orientation:Orientation) :List<float> =
        let GetObsList = 
            match orientation with 
                |ColWise -> 
                    [for i=0 to dataMatrix.NumCols-1 do dataMatrix.Column i ]
                |RowWise -> 
                    let dat = dataMatrix.Transpose
                    [for i=0 to dat.NumCols-1 do dat.Column i]
        
        [for obs in GetObsList -> mahalanobisDistance obs dataMatrix dataSource orientation]