function y=intimage(x)

y = cumsum(cumsum(double(x)),2);