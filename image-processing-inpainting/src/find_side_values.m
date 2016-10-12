function [ values ] = find_side_values(Image, direction, x, y, no_of_values)
[X,Y] = size(Image);

% Check in what direction to find values
% x
if (direction(1) ~= 0)
    d = direction(1);
    length = X;
    line = Image(:, y);
    start = x;
% y
else
    d = direction(2);
    length = Y;
    line = Image(x, :)';
    start = y;
end

values = [];
for k = 1:no_of_values
    % makes sure we can go up and left
    offset = k * d;
    if ((start+offset > length) || (start+offset == 0))
        break;
    end
    values = [ values line(start + offset)];
end

% Default to the pixel value
if (k == 1)
    values = Image(x,y);
end
end

