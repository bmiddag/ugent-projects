function noisy_im = add_noise( im, sigma )
    dims = size(im);
    noise = randn(dims).*sigma;
    % First cast to uint: --> values in range [0,255]
    noisy_im = double(uint8(double(im) + double(noise)));
end

