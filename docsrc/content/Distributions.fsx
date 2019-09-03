(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/FSharp.Stats/netstandard2.0"
#r "../../packages/formatting/FSharp.Plotly/lib/netstandard2.0/Fsharp.Plotly.dll"
#r "netstandard"
open FSharp.Plotly
open FSharp.Plotly.StyleParam
open FSharp.Plotly.Axis

(**
#Probability Distributions


FSharp.Stats provides a wide range of probability distributions. Given the
distribution parameters they can be used to investigate their statistical properties
or to sample non-uniform random numbers.

For every distribution the probability density function (PDF) and cumulative probability function (CDF) can be accessed.
By using the PDF you can access the probability for exacty X=k success states. The CDF is used when the cumulative probabilities of X<=k is required.




<a name="Continuous"></a>

##Continuous

###Normal distribution

The normal or Gaussian distribution is a very common continuous probability distribution.
Due to the central limit theorem, randomly sampled means of a random and independent distribution tend to approximate a normal distribution
It describes the probability, that under a given mean and 
a given dispersion (standard deviation) an event occurs exactly k times. 

It is defined by two parameters N(mu,tau):

  - mu = mean

  - tau = standard deviation

Example: The distribution of bread weights of a local manufacturer follow a normal distribution with mean 500 g and a standard
deviation of 20 g.

NormA: What is the probability of bread weights to be lower than 470 g?

NormB: What is the probability of bread weights to be higher than 505 g?

NormC: Sample independently 10 values from the normal distribution and calculate their mean.

*)

#r "FSharp.Stats.dll"
open FSharp.Stats
open FSharp.Stats.Distributions

// Creates a normal distribution with � = 500 and tau = 20 
let normal = Continuous.normal 500. 20.

// NormA: What is the probability of bread weights to be equal or lower than 470 g?
let normA = normal.CDF 470.
// Output: 0.06681 = 6.68 %

// NormB: What is the probability of bread weights to be higher than 505 g?
let normB = 1. - (normal.CDF 505.)
// Output: 0.401294 = 40.13 %

// NormC: Sample independently 10 values from the normal distribution and calculate their mean.
let normC = 
    Seq.init 100 (fun _ -> normal.Sample())
    |> Seq.mean
// Output: 497.85 g

(***hide***)
let plotNormal =
    let xAxis() = LinearAxis.init(Title="x",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(X=k)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    [400. .. 600.]
    |> List.map (fun x -> x,normal.PDF x)
    |> Chart.Area
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())
    |> Chart.withTitle "N(500,20) PDF"

(*** include-value:plotNormal ***)

(***hide***)
let plotNormalCDF =
    let xAxis() = LinearAxis.init(Title="x",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="cdf(x)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    [400. .. 600.]
    |> List.map (fun x -> x,normal.CDF x)
    |> Chart.Area
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())
    |> Chart.withTitle "N(500,20) CDF"

(*** include-value:plotNormalCDF ***)

(**

###Students t distribution

*)

let studentTParams = [(0.,1.,1.);(0.,1.,2.);(0.,1.,5.);]
let xStudentT = [-10. ..0.1.. 10.]

let pdfStudentT mu tau dof = 
    xStudentT 
    |> List.map (Continuous.StudentT.PDF mu tau dof)
    |> List.zip xStudentT


let cdfStudentT mu tau dof = 
    xStudentT 
    |> List.map (Continuous.StudentT.CDF  mu tau dof)
    |> List.zip xStudentT

(***hide***)
let v =
    let xAxis() = LinearAxis.init(Title="x",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(x)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    studentTParams
    |> List.map (fun (mu,tau,dof) -> Chart.Spline(pdfStudentT mu tau dof,Name=sprintf "mu=%.1f tau=%.1f dof=%.1f" mu tau dof,ShowMarkers=false))
    |> Chart.Combine
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())

(*** include-value:v ***)

(**


<a name="Discrete"></a>

##Discrete

###Binomial distribution

The binomial distribution describes the probability, that under a given success probability and 
a given number of draws an event occurs exactly k times (with replacement). 

It is defined by two parameters B(n,p):

  - n = number of draws

  - p = probability of success

Example: The school bus is late with a probability of 0.10. 

BinoA: What is the probability of running late exactly 5 times during a 30 day month?

BinoB: What is the probability of running late for a maximum of 5 times?

BinoC: What is the probability of running late for at least 5 times?

*)
#r "FSharp.Stats.dll"
open FSharp.Stats
open FSharp.Stats.Distributions

// Creates a binomial distribution with n=30 and p=0.90 
let binomial = Discrete.binomial 0.1 30

// BinoA: What is the probability of running late exactly 5 times during a 30 day month?
let binoA = binomial.PDF 5
// Output: 0.1023 = 10.23 %

// BinoB: What is the probability of running late for a maximum of 5 times?
let binoB = binomial.CDF 5.
// Output: 0.9268 = 92.68 %

// BinoC: What is the probability of running late for at least 5 times?
let binoC = 1. - (binomial.CDF 4.)
// Output: 0.1755 = 17.55 %

(***hide***)
let plotBinomial =
    let xAxis() = LinearAxis.init(Title="number of successes",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(X=k)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    [0..30]
    |> List.map (fun x -> x,binomial.PDF x)
    |> Chart.Column
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())
    |> Chart.withTitle "B(30,0.1)"

(*** include-value:plotBinomial ***)

(**
###Hypergerometric distribution

The hypergeometric distribution describes the probability, that under a given number of success and failure events and 
a given number of draws an event occurs exactly k times (without replacement). 

It is defined by three parameters Hyp(n,s,f):

  - n = number of draws
  
  - s = number of success events

  - f = number of failure events

Example: You participate in a lottery, where you have to choose 6 numbers out of 49. The lottery queen draws 6 numbers randomly, 
where the order does not matter.

HypA: What is the probability that your 6 numbers are right?

HypB: What is the probability that you have at least 3 right ones?

HypC: What is the probability that you have a maximum of 3 right ones? 
*)

// Creates a hypergeometric distribution with n=6, s=6 and f=49-6=43.
//N=count(succes)+count(failure), K=count(success), n=number of draws
let hyper = Discrete.hypergeometric 49 6 6

// HypA: What is the probability that your 6 numbers are right?
let hypA = hyper.PDF 6
// Output: 7.15-08

// HypB: What is the probability that you have at least 3 right ones?
let hypB = 1. - hyper.CDF 2.
// Output: 0.01864 = 1.86 %

// HypC: What is the probability that you have a maximum of 3 right ones? 
let hypC = binomial.CDF 3.
// Output: 0.6474 = 64.74 %

(***hide***)
let plotHyper =
    let xAxis() = LinearAxis.init(Title="number of successes",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(X=k)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    [0..6]
    |> List.map (fun x -> x,hyper.PDF x)
    |> Chart.Column
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())
    //|> Chart.withSize(600.,400.)
    |> Chart.withTitle "Hyp(6,6,43)"


(*** include-value:plotHyper ***)

(**
###Poisson distribution

The poisson distribution describes, what the probability for a number of events is, occuring in a certain time, area, or volume.

It is defined by just one parameters Po(lambda) where lambda is the average rate.

Example: During a year, a forest is struck by a lightning 5.5 times. 

PoA: What is the probability that the lightning strikes exactly 3 times?

PoB: What is the probability that the lightning strikes less than 2 times?

PoC: What is the probability that the lightning strikes more than 7 times?
*)

// Creates a poisson distribution with lambda=  .
let poisson = Discrete.poisson 5.5

// PoA: What is the probability that the lightning strikes exactly 3 times?
let poA = hyper.PDF 3
// Output: 0.01765 = 1.77 %

// PoB: What is the probability that the lightning strikes less than 2 times?
let poB = hyper.CDF 2.
// Output: 0.98136 = 98.14 %

// PoC: What is the probability that the lightning strikes more than 7 times?
let poC = 1. -  binomial.CDF 6.
// Output: 0.025827 = 2.58 %

(***hide***)
let plotPo =
    let xAxis() = LinearAxis.init(Title="number of successes",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(X=k)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    [0..20]
    |> List.map (fun x -> x,poisson.PDF x)
    |> Chart.Column
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())
    //|> Chart.withSize(600.,400.)
    |> Chart.withTitle "Po(5.5)"


(*** include-value:plotPo ***)

(**
###Gamma distribution

*)

let gammaParams = [(1.,2.);(2.,2.);(3.,2.);(5.,1.);(9.0,0.5);(7.5,1.);(0.5,1.);]
let xgamma = [0. ..0.1.. 20.]

let pdfGamma a b = 
    xgamma 
    |> List.map (Continuous.Gamma.PDF a b)
    |> List.zip xgamma


let gammaPlot =
    let xAxis() = LinearAxis.init(Title="x",Range=Range.MinMax(0.,20.),Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(x)",Range=Range.MinMax(0.,0.5),Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    gammaParams
    |> List.map (fun (a,b) -> Chart.Point(pdfGamma a b,Name=sprintf "a=%.1f b=%.1f" a b) )//,ShowMarkers=false))
    |> Chart.Combine
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())

(*** include-value:gammaPlot ***)

let cdfGamma a b = 
    xgamma 
    |> List.map (Continuous.Gamma.CDF a b)
    |> List.zip xgamma

(***hide***)
let gammaCDFPlot=
    let xAxis() = LinearAxis.init(Title="x",Range=Range.MinMax(0.,20.),Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(x)",Range=Range.MinMax(0.,1.),Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    gammaParams
    |> List.map (fun (a,b) -> Chart.Spline(cdfGamma a b,Name=sprintf "a=%.1f b=%.1f" a b) )//,ShowMarkers=false))
    |> Chart.Combine
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())


(*** include-value:gammaCDFPlot ***)

(**

<a name="Empirical"></a>

##Empirical

You can create empirically derived distributions and sample randomly from these.
In this example 100,000 samples from a student t distribution 


*)

// Randomly taken samples; in this case from a gaussian normal distribution.
let sampleDistribution = 
    let template = Continuous.normal 5. 2.
    Seq.init 100000 (fun _ -> template.Sample())

//creates an empirical distribution with bandwidth 0.1
let empiricalDistribution = 
    Empirical.create 0.1 sampleDistribution

(***hide***)
let plotEmpirical =    
    let xAxis() = LinearAxis.init(Title="x",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
    let yAxis() = LinearAxis.init(Title="p(X=k)",Mirror=Mirror.All,Ticks=TickOptions.Inside,Showgrid=false,Showline=true)
  
    empiricalDistribution
    |> Empirical.getZip
    |> Chart.Column
    |> Chart.withX_Axis(xAxis())
    |> Chart.withY_Axis(yAxis())

(*** include-value:plotEmpirical ***)





(**
###Density estimation

*)

let nv = Array.init 1000 (fun _ -> Distributions.Continuous.Normal.Sample 5. 2.)

let xy = KernelDensity.estimate KernelDensity.Kernel.gaussian 1.0 nv

Chart.SplineArea xy
  

(**
<a name="Bandwidth"></a>

##Bandwidth

<a name="Frequency"></a>

##Frequency
*)