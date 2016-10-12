function avgrgb = AVGRGB(im)
    [M, N, C] = size(im);
    R = sum(sum(im(:,:,1)))/(M*N);
    G = sum(sum(im(:,:,2)))/(M*N);
    B = sum(sum(im(:,:,3)))/(M*N);
    avgrgb = [R G B];
end