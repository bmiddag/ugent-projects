function smoothed_image = smooth_image(image, smooth_factor, mask)
        
    % Add padding --> pixels out of bounds to not contribute to smoothing
    padding = floor(smooth_factor/2);
    smooth_factor = padding*2 +1;
    image = padarray(image, [padding padding 0]);
    mask = padarray(mask, [padding padding]);

    % Extract color planes
    R = double(image(:,:,1)).*mask;
    G = double(image(:,:,2)).*mask;
    B = double(image(:,:,3)).*mask;
    
    % Create new smoothed image
    [M,N,D] = size(image);
	values = zeros(M, N, D);    
	pixels = zeros(M, N);
        
	% Build average smoothed image
	for delta_m = -padding:padding
		for delta_n = -padding:padding
			% Shift color planes & mask
			Rshift = circshift(R, [delta_m, delta_n]);
			Gshift = circshift(G, [delta_m, delta_n]);
			Bshift = circshift(B, [delta_m, delta_n]);
			mask_shift = circshift(mask, [delta_m, delta_n]);
            
			% Add to average nominator & denominators
			values(:,:,1) = values(:,:,1) + Rshift;
			values(:,:,2) = values(:,:,2) + Gshift;
			values(:,:,3) = values(:,:,3) + Bshift;
			pixels = pixels + mask_shift;
		end
	end

	% Cut away the padding
	values = values(padding+1:M-padding,padding+1:N-padding,:);
	pixels = pixels(padding+1:M-padding,padding+1:N-padding,:);

	% Calculate the average
	values(:,:,1) = values(:,:,1)./pixels(:,:); 
	values(:,:,2) = values(:,:,2)./pixels(:,:);
	values(:,:,3) = values(:,:,3)./pixels(:,:); 

	smoothed_image = uint8(values);
end

