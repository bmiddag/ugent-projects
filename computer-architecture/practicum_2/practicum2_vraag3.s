lucas:
	cmpl	$1, %eax
	jg	.L2
	movl	$2, %eax
	ret
.L2:
	pushl	%ebp
	movl	%esp, %ebp
	subl	$16, %esp
	#vanaf hier
	movl	8(%ebp), %eax
	subl	$1, %eax
	movl	%eax, (%esp)
	call	lucas
	movl	%eax, -8(%ebp)
	movl	8(%ebp), %eax
	subl	$2, %eax
	movl	%eax, (%esp)
	call	lucas
	movl	%eax, -12(%ebp)
	movl	-12(%ebp), %eax
	addl	%eax, %eax
	addl	-8(%ebp), %eax
	movl	%eax, -4(%ebp)
	#tot hier
	movl	%ebp, %esp
	popl	%ebp
	ret