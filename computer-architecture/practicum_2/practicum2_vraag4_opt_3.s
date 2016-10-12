lucas:
	cmpl	$1, %eax
	jg	.L2
	movl	$2, %eax
	ret
.L2:
	movl	%eax, 4(%esp)
	subl	$8, %esp
	subl	$1, %eax
	call	lucas
	cmpl	$2, %eax
	jle	.L3
	movl	%eax, 4(%esp)
	movl	12(%esp), %eax
	subl	$2, %eax
	call	lucas
	shl	%eax
	addl	4(%esp), %eax
	addl	$8, %esp
	ret
.L3:
	leal	(%eax,%eax,2), %eax
	addl	$8, %esp
	ret