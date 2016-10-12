function mask = detect_scratch(Filename)

alfa = 20;
beta = 35;

Image = imread(Filename);
[Width, Height, ~] = size(Image);

if (ndims(Image) > 2)
    Image = rgb2gray(Image);
end

%% Binary Image Formation
directions = [
    [-1 0]; % left
    [1 0];  % right
    [0 -1]; % up
    [0 1]   % down
    ];

binary_img = ones(Width, Height);
for i = 1:Width
    for j = 1:Height
        sides = [];
        % Loop over the directions
        for d = 1:size(directions, 1)
            side_values = find_side_values(Image, directions(d,:), i, j, 10);
            sides = [sides; mean(side_values)];
        end
        % Use a padded image for ease of use
        x = padarray(Image, [1 1]);
        % Pad the edges with the same values, corners don't matter
        x(1,:) = x(2,:);
        x(:, 1) = x(:, 2);
        x(Width,:) = x(Width-1 ,:);
        x(:, Height) = x(:, Height-1);
        
        a = i+1;
        b = j+1;
        % Create a binary image based on the gradient values between pixels
        f1 = abs(x(a, b) - x(a, b-1)) > alfa; % this - up
        f2 = abs(x(a, b) - x(a, b+1)) > alfa; % this - down
        f3 = abs(sides(1) - sides(2)) < beta; % left - right
        f4 = abs(x(a, b) - x(a-1, b)) > alfa; % this - left
        f5 = abs(x(a, b) - x(a+1, b)) > alfa; % this - right
        f6 = abs(sides(3) - sides(4)) < beta; % up   - down
        
        binary_img(i, j) = ((f1 || f2) && f3) || ((f4 || f5) && f6);
    end
end
binary_img = and(binary_img, edge(Image, 'Canny'));
binary_img = imfill(binary_img, 'holes');

%% Image Rotation based on Hough Transform
no_of_angles = 24;
mask = zeros(Width, Height);
for i = 1:no_of_angles
    angle = (180/no_of_angles) * i;
    rotI = imrotate(binary_img, -angle, 'crop', 'bilinear');

    [H, theta, rho] = hough(rotI);
    P = houghpeaks(H,15,'threshold',floor(max(H(:))/2));
    lines = houghlines(rotI,theta,rho,P,'FillGap',10,'MinLength', 30);
    plotted = plot_lines(lines, Width, Height, angle);
    mask = or(mask,plotted);
end
mask = imdilate(mask, strel('disk', 1));
end

    