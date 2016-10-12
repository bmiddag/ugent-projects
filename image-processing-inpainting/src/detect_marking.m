% Project Image Processing

% Yasser Deceukelier
% Bart Middag
% Titouan Vervack

%% Question 2.1
 
function mask = detect_marking(image, tol, yellow_tol)

    if nargin < 3
        yellow_tol = 10;
        if(nargin < 2)
           tol = 10;
       end
    end

	% Preparation mask
	[M, N, ~] = size(image);
	mask = zeros(M, N);
    
    % Search for most "connected" yellow color
    count_mask = zeros(M, N);
    connected_count = zeros(256, 256, 256);
    for i = 1:M
        for j = 1:N
            if count_mask(i, j) == 0
                % Check if kinda yellow
                pixel = squeeze(image(i, j, :));
                R = double(pixel(1)); G = double(pixel(2)); B = double(pixel(3));
                % if R > 220 && G > 200 && B < (100 -abs(R-G))
                if (pixel(1)-255)^2 <= yellow_tol^2 && (pixel(2)-255)^2 <= yellow_tol^2 && (pixel(3)-0)^2 <= yellow_tol^2
                    connected = magicwand(image, i, j, tol);
                    count = sum(sum(connected));
                    % R, G & B +1 because indexes start from 1
                    % Calculate a quadratic score: larger connected areas => higher scores
                    connected_count(round(R)+1, round(G)+1, round(B)+1) = connected_count(round(R)+1, round(G)+1, round(B)+1) + count^2;
                    count_mask(connected) = connected_count(connected) + count;
                end
            end
        end
    end
    
    % Assume most "connected" yellow is the marking color
    [~, idx] = max(connected_count(:));
    [yr,yg,yb] = ind2sub(size(connected_count), idx);
    % All -1: convert from index to R, G, B values
    yr = yr -1; yg = yg-1; yb = yb-1;
    
	% For each pixel
	for i = 1:M
        for j = 1:N
            % If pixel isn't already part of the mask
            if mask(i, j) == 0
                % Check if pixel has marking color
                pixel = squeeze(image(i, j, :));
                if (pixel(1)-yr)^2 <= tol^2 && (pixel(2)-yg)^2 <= tol^2 && (pixel(3)-yb)^2 <= tol^2
                    
                    % Check if multiple neigbouring pixel have this exact 
                    % same value. If this is not the case, the pixel 
                    % probably isn't part of the marking, but instead is
                    % part of the image itself
                    if count_mask(i, j) >= 4
                        mask(i,j) = 1;
                     end
                end
            end
        end 
	end

    % Add a 1 pixel edge
    edges = zeros(M+2, N+2);    % Make 1 pixel bigger on each side to avoid checks in the loop body 
    for i = 1:M
        for j = 1:N
            if mask(i, j) == 1
                edges(i:i+2, j:j+2) = 1;
            end
        end
    end

    mask = mask | edges(2:M+1, 2:N+1);

end